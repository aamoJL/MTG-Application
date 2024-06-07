using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardCollection;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.UseCases;
using MTGApplication.General.Services.IOService;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollection;

public partial class CardCollectionViewModel(ICardAPI<MTGCard> cardAPI) : ViewModelBase, IWorker, ISavable
{
  [ObservableProperty] private MTGCardCollection collection = new();
  [ObservableProperty] private MTGCardCollectionList selectedList;
  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public QueryCardsViewModel QueryCardsViewModel { get; } = new(cardAPI);
  public CardCollectionConfirmers Confirmers { get; init; } = new();
  public Notifier Notifier { get; init; } = new();
  public ClipboardService ClipboardService { get; init; } = new();
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
      CardCollectionConfirmers.GetLoadCollectionConfirmation(await new GetCardCollectionNames(Repository).Execute()))
      is not string loadName)
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

  [RelayCommand(CanExecute = nameof(CanExecuteEditListCommand))]
  private async Task EditList()
  {
    if (!EditListCommand.CanExecute(null)) return;

    if (await Confirmers.EditCollectionListConfirmer.Confirm(
      CardCollectionConfirmers.GetEditCollectionListConfirmation((SelectedList.Name, SelectedList.SearchQuery)))
      is not (string name, string query) args)
      return;

    if (string.IsNullOrEmpty(name))
      new SendNotification(Notifier).Execute(CardCollectionNotifications.EditListNameError);
    else if (string.IsNullOrEmpty(query))
      new SendNotification(Notifier).Execute(CardCollectionNotifications.EditListQueryError);
    else if (SelectedList.Name != name && Collection.CollectionLists.FirstOrDefault(x => x.Name == name) is not null)
      new SendNotification(Notifier).Execute(CardCollectionNotifications.EditListExistsError);
    else
    {
      SelectedList.Name = name;
      SelectedList.SearchQuery = query;
      HasUnsavedChanges = true;

      await ((IWorker)this).DoWork(QueryCardsViewModel.UpdateQueryCards(SelectedList?.SearchQuery ?? string.Empty));

      new SendNotification(Notifier).Execute(CardCollectionNotifications.EditListSuccess);
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteDeleteListCommand))]
  private async Task DeleteList()
  {
    if (!DeleteListCommand.CanExecute(null)) return;

    if (await Confirmers.DeleteCollectionListConfirmer.Confirm(
      CardCollectionConfirmers.GetDeleteCollectionListConfirmation(SelectedList.Name))
      is not ConfirmationResult.Yes)
      return;

    if (Collection.CollectionLists.Remove(SelectedList))
    {
      HasUnsavedChanges = true;

      await SelectList(Collection.CollectionLists.FirstOrDefault()?.Name);

      new SendNotification(Notifier).Execute(CardCollectionNotifications.DeleteListSuccess);
    }
    else
      new SendNotification(Notifier).Execute(CardCollectionNotifications.DeleteListError);
  }

  [RelayCommand(CanExecute = nameof(CanExecuteImportCardsCommand))]
  private async Task ImportCards()
  {
    if (!ImportCardsCommand.CanExecute(null)) return;

    if (await Confirmers.ImportCardsConfirmer.Confirm(
      CardCollectionConfirmers.GetImportCardsConfirmation())
      is not string importText || string.IsNullOrEmpty(importText))
      return;

    var result = await ((IWorker)this).DoWork(new ImportCards(CardAPI).Execute(importText));
    var addedCards = result.Found.Where(found => !SelectedList.Cards.Any(old => old.Info.ScryfallId == found.Info.ScryfallId))
      .DistinctBy(x => x.Info.ScryfallId).ToList();

    foreach (var card in addedCards)
      SelectedList.Cards.Add(card);

    HasUnsavedChanges = true;

    if (result.Found.Length == 0)
      new SendNotification(Notifier).Execute(CardCollectionNotifications.ImportCardsError);
    else
      new SendNotification(Notifier).Execute(CardCollectionNotifications.ImportCardsSuccessOrWarning(
        added: addedCards.Count,
        skipped: result.Found.Length - addedCards.Count,
        notFound: result.NotFoundCount));
  }

  [RelayCommand(CanExecute = nameof(CanExecuteExportCardsCommand))]
  private async Task ExportCards()
  {
    if (!ExportCardsCommand.CanExecute(null)) return;

    if (await Confirmers.ExportCardsConfirmer.Confirm(
      CardCollectionConfirmers.GetExportCardsConfirmation(new ExportCardsById().Execute(SelectedList.Cards)))
      is not string response || string.IsNullOrEmpty(response))
      return;

    ClipboardService.CopyToClipboard(response);
    new SendNotification(Notifier).Execute(ClipboardService.CopiedNotification);
  }

  [RelayCommand]
  private async Task SelectList(string name)
  {
    SelectedList = Collection.CollectionLists.FirstOrDefault(x => x.Name == name);
    QueryCardsViewModel.OwnedCards = SelectedList?.Cards ?? [];

    OnPropertyChanged(nameof(SelectedListCardCount));
    await ((IWorker)this).DoWork(QueryCardsViewModel.UpdateQueryCards(SelectedList?.SearchQuery ?? string.Empty));
  }

  [RelayCommand]
  private async Task ShowCardPrints(MTGCard card)
  {
    var prints = (await ((IWorker)this).DoWork(CardAPI.FetchFromUri(pageUri: card.Info.PrintSearchUri, paperOnly: true, fetchAll: true))).Found;

    await Confirmers.ShowCardPrintsConfirmer.Confirm(CardCollectionConfirmers.GetShowCardPrintsConfirmation(prints));
  }

  [RelayCommand(CanExecute = nameof(CanExecuteSwitchCardOwnershipCommand))]
  private void SwitchCardOwnership(MTGCard card)
  {
    if (!SwitchCardOwnershipCommand.CanExecute(card)) return;

    if (SelectedList.Cards.FirstOrDefault(x => x.Info.ScryfallId == card.Info.ScryfallId) is MTGCard existingCard)
      SelectedList.Cards.Remove(existingCard);
    else
      SelectedList.Cards.Add(card);

    HasUnsavedChanges = true;

    OnPropertyChanged(nameof(SelectedListCardCount));
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

  private bool CanExecuteEditListCommand() => SelectedList != null;

  private bool CanExecuteDeleteListCommand() => SelectedList != null;

  private bool CanExecuteImportCardsCommand() => SelectedList != null;

  private bool CanExecuteExportCardsCommand() => SelectedList != null;

  private bool CanExecuteSwitchCardOwnershipCommand(MTGCard card) => card != null && SelectedList != null;

  private async Task SetCollection(MTGCardCollection collection)
  {
    Collection = collection;
    HasUnsavedChanges = false;

    await SelectList(Collection.CollectionLists.FirstOrDefault()?.Name);
  }
}