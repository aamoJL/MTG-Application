using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardCollection;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionViewModel(ICardAPI<MTGCard> cardAPI) : ViewModelBase, IWorker, ISavable
{
  [ObservableProperty] private MTGCardCollection collection = new();
  [ObservableProperty,
    NotifyPropertyChangedFor(nameof(SelectedListCardCount))]
  private MTGCardCollectionList selectedList;
  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public IncrementalLoadingCardCollection<CardCollectionMTGCard> QueryCards { get; } = new(new CardCollectionIncrementalCardSource(cardAPI));
  public CardCollectionConfirmers Confirmers { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public IRepository<MTGCardCollectionDTO> Repository { get; init; } = new CardCollectionDTORepository();

  public int SelectedListCardCount => SelectedList?.Cards.Count ?? 0;

  private ICardAPI<MTGCard> CardAPI { get; } = cardAPI;

  [RelayCommand]
  private async Task NewCollection()
  {
    if (!await ConfirmUnsavedChanges())
      return;

    await SetCollection(new());
  }

  [RelayCommand]
  private async Task OpenCollection()
  {
    if (!await ConfirmUnsavedChanges())
      return;

    if (await Confirmers.LoadCollectionConfirmer.Confirm(
      CardCollectionConfirmers.GetLoadCollectionConfirmation(
        await new GetCardCollectionNames(Repository).Execute())) is not string loadName)
      return;

    if (await ((IWorker)this).DoWork(new LoadCardCollection(Repository, CardAPI).Execute(loadName)) is MTGCardCollection loadedCollection)
    {
      await SetCollection(loadedCollection);
      new SendNotification(Notifier).Execute(CardCollectionNotifications.OpenCollectionSuccess);
    }
    else new SendNotification(Notifier).Execute(CardCollectionNotifications.OpenCollectionError);
  }

  [RelayCommand(CanExecute = nameof(CanExecuteSaveCollectionCommand))]
  private async Task SaveCollection()
  {
    if (!SaveCollectionCommand.CanExecute(null)) return;

    var oldName = Collection.Name;
    var overrideOld = false;
    var saveName = await Confirmers.SaveCollectionConfirmer.Confirm(
      CardCollectionConfirmers.GetSaveCollectionConfirmation(oldName));

    if (string.IsNullOrEmpty(saveName))
      return;

    // Override confirmation
    if (saveName != oldName && await new CardCollectionExists(Repository).Execute(saveName))
    {
      switch (await Confirmers.OverrideCollectionConfirmer.Confirm(CardCollectionConfirmers.GetOverrideCollectionConfirmation(saveName)))
      {
        case ConfirmationResult.Yes: overrideOld = true; break;
        case ConfirmationResult.No:
        default: return; // Cancel
      }
    }

    switch (await ((IWorker)this).DoWork(new SaveCardCollection(Repository).Execute(new(Collection, saveName, overrideOld))))
    {
      case true:
        new SendNotification(Notifier).Execute(CardCollectionNotifications.SaveCollectionSuccess);
        HasUnsavedChanges = false;
        break;
      case false:
        new SendNotification(Notifier).Execute(CardCollectionNotifications.SaveCollectionError);
        break;
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteDeleteCollectionCommand))]
  private async Task DeleteCollection()
  {
    if (!DeleteCollectionCommand.CanExecute(null))
      return;

    var deleteConfirmationResult = await Confirmers.DeleteCollectionConfirmer.Confirm(
      CardCollectionConfirmers.GetDeleteCollectionConfirmation(Collection.Name));

    switch (deleteConfirmationResult)
    {
      case ConfirmationResult.Yes: break;
      default: return; // Cancel
    }

    switch (await ((IWorker)this).DoWork(new DeleteCardCollection(Repository).Execute(Collection)))
    {
      case true:
        await SetCollection(new());
        new SendNotification(Notifier).Execute(CardCollectionNotifications.DeleteCollectionSuccess);
        break;
      case false:
        new SendNotification(Notifier).Execute(CardCollectionNotifications.DeletecollectionError);
        break;
    }
  }

  [RelayCommand]
  private async Task NewList()
  {
    if (await Confirmers.NewCollectionListConfirmer.Confirm(CardCollectionConfirmers.GetNewCollectionListConfirmation())
      is not (string name, string query))
      return;

    if (string.IsNullOrEmpty(name))
      new SendNotification(Notifier).Execute(CardCollectionNotifications.NewListNameError);
    else if (string.IsNullOrEmpty(query))
      new SendNotification(Notifier).Execute(CardCollectionNotifications.NewListQueryError);
    else if (Collection.CollectionLists.FirstOrDefault(x => x.Name == name) is not null)
      new SendNotification(Notifier).Execute(CardCollectionNotifications.NewListExistsError);
    else
    {
      Collection.CollectionLists.Add(new MTGCardCollectionList() { Name = name, SearchQuery = query });
      HasUnsavedChanges = true;

      await SelectList(name);

      new SendNotification(Notifier).Execute(CardCollectionNotifications.NewListSuccess);
    }
  }

  [RelayCommand]
  private async Task EditList()
  {
    // TODO: EditList
  }

  [RelayCommand]
  private async Task ImportCards()
  {
    // TODO: ImportCards
  }

  [RelayCommand]
  private async Task ExportCards()
  {
    // TODO: ExportCards
  }

  [RelayCommand]
  private async Task DeleteList()
  {
    // TODO: DeleteList
  }

  [RelayCommand]
  private async Task SelectList(string name)
  {
    SelectedList = Collection.CollectionLists.FirstOrDefault(x => x.Name == name);

    var searchResult = await ((IWorker)this).DoWork(new GetMTGCardsBySearchQuery(CardAPI).Execute(SelectedList?.SearchQuery ?? string.Empty));
    QueryCards.SetCollection([.. searchResult.Found], searchResult.NextPageUri, searchResult.TotalCount);
  }

  public async Task<bool> ConfirmUnsavedChanges()
  {
    if (!HasUnsavedChanges || !SaveCollectionCommand.CanExecute(null))
      return true;

    switch (await Confirmers.SaveUnsavedChangesConfirmer
      .Confirm(CardCollectionConfirmers.GetSaveUnsavedChangesConfirmation(Collection.Name)))
    {
      case ConfirmationResult.Yes: await SaveCollection(); return !HasUnsavedChanges;
      case ConfirmationResult.No: return true;
      default: return false;
    };
  }

  private bool CanExecuteSaveCollectionCommand() => Collection.CollectionLists.Any();

  private bool CanExecuteDeleteCollectionCommand() => !string.IsNullOrEmpty(Collection.Name);

  private async Task SetCollection(MTGCardCollection collection)
  {
    Collection = collection;
    HasUnsavedChanges = false;

    await SelectList(Collection.CollectionLists.FirstOrDefault()?.Name);
  }
}