using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardCollection;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

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
  public IRepository<MTGCardCollectionDTO> Repository { get; init; } = new CardCollectionDTORepository();

  public int SelectedListCardCount => SelectedList?.Cards.Count ?? 0;

  private ICardAPI<MTGCard> CardAPI { get; } = cardAPI;

  [RelayCommand]
  private async Task NewCollection()
  {
    if (!await ConfirmUnsavedChanges())
      return;

    Collection = new();
    HasUnsavedChanges = false;
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
      HasUnsavedChanges = false;
      Collection = loadedCollection;
      SelectedList = Collection.CollectionLists.FirstOrDefault();

      var searchResult = await ((IWorker)this).DoWork(new GetMTGCardsBySearchQuery(CardAPI).Execute(SelectedList?.SearchQuery ?? string.Empty));
      QueryCards.SetCollection([.. searchResult.Found], searchResult.NextPageUri, searchResult.TotalCount);

      // TODO: RaiseInAppNotification(NotificationService.NotificationType.Success, "The collection was loaded successfully.");
    }
    else
    {
      // TODO: RaiseInAppNotification(NotificationService.NotificationType.Error, "Error. Could not load the collection.")
    }
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
        //TODO: new SendNotification(Notifier).Execute(DeckEditorNotifications.SaveSuccessNotification);
        HasUnsavedChanges = false;
        break;
      case false:
        //TODO: new SendNotification(Notifier).Execute(DeckEditorNotifications.SaveErrorNotification); 
        break;
    }
  }

  [RelayCommand]
  private async Task DeleteCollection()
  {
    // TODO: DeleteCollection
  }

  [RelayCommand]
  private async Task NewList()
  {
    // TODO: NewList
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

  public async Task<bool> ConfirmUnsavedChanges()
  {
    if (!HasUnsavedChanges) return true;

    switch (await Confirmers.SaveUnsavedChangesConfirmer
      .Confirm(CardCollectionConfirmers.GetSaveUnsavedChangesConfirmation(Collection.Name)))
    {
      case ConfirmationResult.Yes: await SaveCollection(); return !HasUnsavedChanges;
      case ConfirmationResult.No: return true;
      default: return false;
    };
  }

  private bool CanExecuteSaveCollectionCommand() => Collection.CollectionLists.Any();
}