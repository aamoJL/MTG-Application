using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.DeckRepository;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;
public partial class DeckEditorViewModel : ViewModelBase, ISavable, IWorker
{
  private MTGCardDeck deck = new();

  public DeckEditorViewModel()
  {
    DeckCardList = new(CardAPI)
    {
      Cards = Deck.DeckCards,
      OnChange = () =>
      {
        HasUnsavedChanges = true;
        OnPropertyChanged(nameof(DeckSize));
      },
      UndoStack = UndoStack,
      Worker = this,
      Confirmers = DeckEditorConfirmers.CardListConfirmers,
      Notifier = Notifier,
    };
    MaybeCardList = new(CardAPI)
    {
      Cards = Deck.Maybelist,
      OnChange = () => { HasUnsavedChanges = true; },
      UndoStack = UndoStack,
      Worker = this,
      Confirmers = DeckEditorConfirmers.CardListConfirmers,
      Notifier = Notifier,
    };
    WishCardList = new(CardAPI)
    {
      Cards = Deck.Wishlist,
      OnChange = () => { HasUnsavedChanges = true; },
      UndoStack = UndoStack,
      Worker = this,
      Confirmers = DeckEditorConfirmers.CardListConfirmers,
      Notifier = Notifier,
    };
    RemoveCardList = new(CardAPI)
    {
      Cards = Deck.Removelist,
      OnChange = () => { HasUnsavedChanges = true; },
      UndoStack = UndoStack,
      Worker = this,
      Confirmers = DeckEditorConfirmers.CardListConfirmers,
      Notifier = Notifier,
    };
  }

  public DeckEditorViewModel(MTGCardDeck deck) : this() => Deck = deck;

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

      UndoStack.Clear();
      HasUnsavedChanges = false;

      OnPropertyChanged(nameof(DeckSize));
      // Can't invoke changed event for the name if the name was already empty because visual state binding would break.
      if (DeckName != oldName) OnPropertyChanged(nameof(DeckName));
      OnPropertyChanged(nameof(Commander));
      OnPropertyChanged(nameof(Partner));
      SaveDeckCommand.NotifyCanExecuteChanged();
      DeleteDeckCommand.NotifyCanExecuteChanged();
    }
  }
  private ReversibleCommandStack UndoStack { get; } = new();

  public ICardAPI<MTGCard> CardAPI { private get; init; } = App.MTGCardAPI;
  public IRepository<MTGCardDeckDTO> Repository { private get; init; } = new DeckDTORepository();

  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public DeckEditorConfirmers DeckEditorConfirmers { get; init; } = new();
  public DeckEditorNotifier Notifier { get; init; } = new();
  public CardFilters CardFilters { get; init; } = new();
  public CardSorter CardSorter { get; init; } = new();

  public CardListViewModel DeckCardList { get; }
  public CardListViewModel MaybeCardList { get; }
  public CardListViewModel WishCardList { get; }
  public CardListViewModel RemoveCardList { get; }

  public MTGCard Commander
  {
    get => Deck.Commander;
    set
    {
      Deck.Commander = value;
      OnPropertyChanged(nameof(Commander));
    }
  }
  public MTGCard Partner
  {
    get => Deck.CommanderPartner;
    set
    {
      Deck.CommanderPartner = value;
      OnPropertyChanged(nameof(Partner));
    }
  }
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
      new SendNotification(Notifier).Execute(DeckEditorNotifier.DeckEditorNotifications.LoadSuccessNotification);
    }
    else
      new SendNotification(Notifier).Execute(DeckEditorNotifier.DeckEditorNotifications.LoadErrorNotification);
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
        new SendNotification(Notifier).Execute(DeckEditorNotifier.DeckEditorNotifications.SaveSuccessNotification);
        OnPropertyChanged(nameof(DeckName));
        HasUnsavedChanges = false;
        break;
      case false: new SendNotification(Notifier).Execute(DeckEditorNotifier.DeckEditorNotifications.SaveErrorNotification); break;
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
        new SendNotification(Notifier).Execute(DeckEditorNotifier.DeckEditorNotifications.DeleteSuccessNotification); break;
      case false: new SendNotification(Notifier).Execute(DeckEditorNotifier.DeckEditorNotifications.DeleteErrorNotification); break;
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteUndoCommand))] private void Undo() => UndoStack.Undo();

  [RelayCommand(CanExecute = nameof(CanExecuteRedoCommand))] private void Redo() => UndoStack.Redo();

  private bool CanExecuteSaveDeckCommand() => Deck.DeckCards.Count > 0;

  private bool CanExecuteDeleteDeckCommand() => !string.IsNullOrEmpty(Deck.Name);

  private bool CanExecuteOpenDeckCommand(string name) => name != string.Empty;

  private bool CanExecuteUndoCommand() => UndoStack.CanUndo;

  private bool CanExecuteRedoCommand() => UndoStack.CanRedo;
}