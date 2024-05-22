using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.DeckRepository;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.IOService;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor;
public partial class DeckEditorViewModel : ViewModelBase, ISavable, IWorker
{
  private MTGCardDeck deck = new();

  public DeckEditorViewModel(ICardAPI<MTGCard> cardAPI, MTGCardDeck deck = null, Notifier notifier = null, DeckEditorConfirmers confirmers = null)
  {
    CardAPI = cardAPI;
    Notifier = notifier ?? new();
    DeckEditorConfirmers = confirmers ?? new();

    DeckCardList = CreateCardListViewModel(Deck.DeckCards, () =>
    {
      HasUnsavedChanges = true;
      OnPropertyChanged(nameof(DeckSize));
    });
    MaybeCardList = CreateCardListViewModel(Deck.Maybelist, () => { HasUnsavedChanges = true; });
    WishCardList = CreateCardListViewModel(Deck.Wishlist, () => { HasUnsavedChanges = true; });
    RemoveCardList = CreateCardListViewModel(Deck.Removelist, () => { HasUnsavedChanges = true; });

    CommanderViewModel = CreateCommanderViewModel(changeAction: new()
    {
      Action = (newCard) => { Deck.Commander = newCard; CommanderViewModel.Card = newCard; HasUnsavedChanges = true; },
      ReverseAction = (oldCard) => { Deck.Commander = oldCard; CommanderViewModel.Card = oldCard; HasUnsavedChanges = true; }
    });
    PartnerViewModel = CreateCommanderViewModel(changeAction: new()
    {
      Action = (newCard) => { Deck.CommanderPartner = newCard; PartnerViewModel.Card = newCard; HasUnsavedChanges = true; },
      ReverseAction = (oldCard) => { Deck.CommanderPartner = oldCard; PartnerViewModel.Card = oldCard; HasUnsavedChanges = true; }
    });

    Deck = deck ?? new();
  }

  private MTGCardDeck Deck
  {
    get => deck;
    set
    {
      var oldName = deck.Name;

      deck = value;

      DeckCardList.Cards = deck.DeckCards;
      MaybeCardList.Cards = deck.Maybelist;
      WishCardList.Cards = deck.Wishlist;
      RemoveCardList.Cards = deck.Removelist;

      CommanderViewModel.Card = deck.Commander;
      PartnerViewModel.Card = deck.CommanderPartner;

      UndoStack.Clear();
      HasUnsavedChanges = false;

      OnPropertyChanged(nameof(DeckSize));
      // Can't invoke changed event for the name if the name was already empty because visual state binding would break.
      if (DeckName != oldName) OnPropertyChanged(nameof(DeckName));
      SaveDeckCommand.NotifyCanExecuteChanged();
      DeleteDeckCommand.NotifyCanExecuteChanged();
    }
  }
  private ReversibleCommandStack UndoStack { get; } = new();
  private ICardAPI<MTGCard> CardAPI { get; }

  public DeckEditorConfirmers DeckEditorConfirmers { get; }
  public Notifier Notifier { get; }
  public CardListViewModel DeckCardList { get; }
  public CardListViewModel MaybeCardList { get; }
  public CardListViewModel WishCardList { get; }
  public CardListViewModel RemoveCardList { get; }
  public CommanderViewModel CommanderViewModel { get; }
  public CommanderViewModel PartnerViewModel { get; }

  public IRepository<MTGCardDeckDTO> Repository { get; init; } = new DeckDTORepository();
  public CardFilters CardFilters { get; init; } = new();
  public CardSorter CardSorter { get; init; } = new();

  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public string DeckName => Deck.Name;
  public int DeckSize => Deck.DeckSize;

  public async Task<bool> ConfirmUnsavedChanges()
  {
    if (!HasUnsavedChanges) return true;

    switch (await DeckEditorConfirmers.SaveUnsavedChangesConfirmer
      .Confirm(DeckEditorConfirmers.GetSaveUnsavedChangesConfirmation(DeckName)))
    {
      case ConfirmationResult.Yes: await SaveDeck(); return !HasUnsavedChanges;
      case ConfirmationResult.No: return true;
      default: return false;
    };
  }

  private CardListViewModel CreateCardListViewModel(ObservableCollection<MTGCard> cards, Action onChange)
  {
    return new(CardAPI)
    {
      Cards = cards,
      OnChange = onChange,
      UndoStack = UndoStack,
      Worker = this,
      Confirmers = DeckEditorConfirmers.CardListConfirmers,
      Notifier = Notifier
    };
  }

  private CommanderViewModel CreateCommanderViewModel(ReversibleAction<MTGCard> changeAction)
  {
    return new(CardAPI)
    {
      UndoStack = UndoStack,
      Notifier = Notifier,
      Worker = this,
      ReversibleChange = changeAction
    };
  }
}

public partial class DeckEditorViewModel
{
  [RelayCommand]
  private async Task NewDeck()
  {
    if (await ConfirmUnsavedChanges())
      Deck = new();
  }

  [RelayCommand(CanExecute = nameof(CanExecuteOpenDeckCommand))]
  private async Task OpenDeck(string loadName = null)
  {
    if (!OpenDeckCommand.CanExecute(loadName) || !await ConfirmUnsavedChanges())
      return;

    loadName ??= await DeckEditorConfirmers.LoadDeckConfirmer
      .Confirm(DeckEditorConfirmers.GetLoadDeckConfirmation(await ((IWorker)this).DoWork(new GetDeckNames(Repository).Execute())));

    if (string.IsNullOrEmpty(loadName))
      return;

    if (await ((IWorker)this).DoWork(new LoadDeck(Repository, CardAPI).Execute(loadName)) is MTGCardDeck deck)
    {
      Deck = deck;
      new SendNotification(Notifier).Execute(DeckEditorNotifications.LoadSuccessNotification);
    }
    else
      new SendNotification(Notifier).Execute(DeckEditorNotifications.LoadErrorNotification);
  }

  [RelayCommand(CanExecute = nameof(CanExecuteSaveDeckCommand))]
  private async Task SaveDeck()
  {
    if (!SaveDeckCommand.CanExecute(null)) return;

    var oldName = DeckName;
    var overrideOld = false;
    var saveName = await DeckEditorConfirmers.SaveDeckConfirmer.Confirm(
      DeckEditorConfirmers.GetSaveDeckConfirmation(DeckName));

    if (string.IsNullOrEmpty(saveName))
      return;

    // Override confirmation
    if (saveName != oldName && await new DeckExists(Repository).Execute(saveName))
    {
      switch (await DeckEditorConfirmers.OverrideDeckConfirmer.Confirm(DeckEditorConfirmers.GetOverrideDeckConfirmation(saveName)))
      {
        case ConfirmationResult.Yes: overrideOld = true; break;
        case ConfirmationResult.Cancel:
        default: return; // Cancel
      }
    }

    switch (await ((IWorker)this).DoWork(new SaveDeck(Repository).Execute(new(Deck, saveName, overrideOld))))
    {
      case true:
        new SendNotification(Notifier).Execute(DeckEditorNotifications.SaveSuccessNotification);
        OnPropertyChanged(nameof(DeckName));
        HasUnsavedChanges = false;
        break;
      case false: new SendNotification(Notifier).Execute(DeckEditorNotifications.SaveErrorNotification); break;
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteDeleteDeckCommand))]
  private async Task DeleteDeck()
  {
    if (!DeleteDeckCommand.CanExecute(null))
      return;

    var deleteConfirmationResult = await DeckEditorConfirmers.DeleteDeckConfirmer.Confirm(
      DeckEditorConfirmers.GetDeleteDeckConfirmation(DeckName));

    switch (deleteConfirmationResult)
    {
      case ConfirmationResult.Yes: break;
      default: return; // Cancel
    }

    switch (await ((IWorker)this).DoWork(new DeleteDeck(Repository).Execute(Deck)))
    {
      case true:
        Deck = new();
        new SendNotification(Notifier).Execute(DeckEditorNotifications.DeleteSuccessNotification); break;
      case false: new SendNotification(Notifier).Execute(DeckEditorNotifications.DeleteErrorNotification); break;
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteUndoCommand))] private void Undo() => UndoStack.Undo();

  [RelayCommand(CanExecute = nameof(CanExecuteRedoCommand))] private void Redo() => UndoStack.Redo();

  /// <summary>
  /// Opens card's Cardmarket page in web browser
  /// </summary>    
  [RelayCommand]
  public async Task OpenEDHRECWebsiteCommand()
    => await NetworkService.OpenUri(EdhrecAPI.GetCommanderWebsiteUri(CommanderViewModel.Card, PartnerViewModel.Card));

  private bool CanExecuteSaveDeckCommand() => Deck.DeckCards.Count > 0;

  private bool CanExecuteDeleteDeckCommand() => !string.IsNullOrEmpty(Deck.Name);

  private bool CanExecuteOpenDeckCommand(string name) => name != string.Empty;

  private bool CanExecuteUndoCommand() => UndoStack.CanUndo;

  private bool CanExecuteRedoCommand() => UndoStack.CanRedo;
}