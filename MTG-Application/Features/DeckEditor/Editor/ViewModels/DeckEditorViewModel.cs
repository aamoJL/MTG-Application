using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.Services.DeckEditor;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.DeckEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor;
public partial class DeckEditorViewModel : ViewModelBase, ISavable, IWorker
{
  private DeckEditorMTGDeck deck = new();

  public DeckEditorViewModel(MTGCardImporter importer, DeckEditorMTGDeck deck = null, Notifier notifier = null, DeckEditorConfirmers confirmers = null)
  {
    Importer = importer;
    Notifier = notifier ?? new();
    Confirmers = confirmers ?? new();

    DeckCardList = CreateCardListViewModel(Deck.DeckCards);
    MaybeCardList = CreateCardListViewModel(Deck.Maybelist);
    WishCardList = CreateCardListViewModel(Deck.Wishlist);
    RemoveCardList = CreateCardListViewModel(Deck.Removelist);

    CommanderViewModel = CreateCommanderViewModel(modelChangeAction: (card) => { Deck.Commander = card; });
    PartnerViewModel = CreateCommanderViewModel(modelChangeAction: (card) => { Deck.CommanderPartner = card; });

    Deck = deck ?? new();
  }

  public DeckEditorMTGDeck Deck
  {
    get => deck;
    set
    {
      var oldName = deck.Name;

      // TODO: change to own method
      deck = value;

      DeckCardList.Cards = deck.DeckCards;
      MaybeCardList.Cards = deck.Maybelist;
      WishCardList.Cards = deck.Wishlist;
      RemoveCardList.Cards = deck.Removelist;

      CommanderViewModel.Card = deck.Commander;
      PartnerViewModel.Card = deck.CommanderPartner;

      UndoStack.Clear();
      HasUnsavedChanges = false;

      // Can't invoke changed event for the name if the name was already empty because visual state binding would break.
      if (DeckName != oldName) OnPropertyChanged(nameof(DeckName));
      OnPropertyChanged(nameof(DeckSize));
      OnPropertyChanged(nameof(DeckPrice));
      SaveDeckCommand.NotifyCanExecuteChanged();
      DeleteDeckCommand.NotifyCanExecuteChanged();
      OpenEdhrecCommanderWebsiteCommand.NotifyCanExecuteChanged();
      ShowDeckTokensCommand.NotifyCanExecuteChanged();
    }
  }
  public IRepository<MTGCardDeckDTO> Repository { get; init; } = new DeckDTORepository();
  public ReversibleCommandStack UndoStack { get; } = new();
  public CardFilters CardFilters { get; init; } = new();
  public CardSorter CardSorter { get; init; } = new();
  public DeckEditorConfirmers Confirmers { get; }
  public Notifier Notifier { get; } = new();
  public MTGCardImporter Importer { get; }
  public CardListViewModel DeckCardList { get; }
  public CardListViewModel MaybeCardList { get; }
  public CardListViewModel WishCardList { get; }
  public CardListViewModel RemoveCardList { get; }
  public CommanderViewModel CommanderViewModel { get; }
  public CommanderViewModel PartnerViewModel { get; }

  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public string DeckName => Deck.Name;
  public int DeckSize => Deck.DeckSize;
  public double DeckPrice => Deck.DeckPrice;
  public IWorker Worker => this;

  public IAsyncRelayCommand<ISavable.ConfirmArgs> ConfirmUnsavedChangesCommand => new ConfirmUnsavedChanges(this).Command;
  public IAsyncRelayCommand NewDeckCommand => new NewDeck(this).Command;
  public IAsyncRelayCommand<string> OpenDeckCommand => new OpenDeck(this).Command;
  public IAsyncRelayCommand SaveDeckCommand => new SaveDeck(this).Command;
  public IAsyncRelayCommand DeleteDeckCommand => new DeleteDeck(this).Command;
  public IRelayCommand UndoCommand => new Undo(this).Command;
  public IRelayCommand RedoCommand => new Redo(this).Command;
  public IAsyncRelayCommand OpenEdhrecCommanderWebsiteCommand => new OpenEdhrecCommanderWebsite(this).Command;
  public IAsyncRelayCommand ShowDeckTokensCommand => new ShowDeckTokens(this).Command;

  [RelayCommand]
  private async Task OpenEdhrecSearchWindow()
  {
    // TODO: open EDHREC search window use case
  }

  [RelayCommand]
  private async Task OpenPlaytestWindow()
  {
    // TODO: open testing window use case
  }

  private CardListViewModel CreateCardListViewModel(ObservableCollection<DeckEditorMTGCard> cards)
  {
    return new(Importer)
    {
      Cards = cards,
      OnChange = () =>
      {
        HasUnsavedChanges = true;
        OnPropertyChanged(nameof(DeckSize));
        OnPropertyChanged(nameof(DeckPrice));
        ShowDeckTokensCommand.NotifyCanExecuteChanged();
      },
      UndoStack = UndoStack,
      Worker = this,
      Confirmers = Confirmers.CardListConfirmers,
      Notifier = Notifier
    };
  }

  private CommanderViewModel CreateCommanderViewModel(Action<DeckEditorMTGCard> modelChangeAction)
  {
    var commanderVM = new CommanderViewModel(Importer)
    {
      UndoStack = UndoStack,
      Notifier = Notifier,
      Confirmers = Confirmers.CommanderConfirmers,
      OnChange = (card) =>
      {
        modelChangeAction?.Invoke(card);
        HasUnsavedChanges = true;
        OnPropertyChanged(nameof(DeckSize));
        OnPropertyChanged(nameof(DeckPrice));
        OpenEdhrecCommanderWebsiteCommand.NotifyCanExecuteChanged();
        ShowDeckTokensCommand.NotifyCanExecuteChanged();
      },
      Worker = this,
    };

    return commanderVM;
  }
}