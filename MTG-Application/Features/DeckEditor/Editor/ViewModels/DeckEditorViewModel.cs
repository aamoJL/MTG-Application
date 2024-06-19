using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.Editor.Services.Converters;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.Editor.UseCases.DeckEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels;
public partial class DeckEditorViewModel : ViewModelBase, ISavable, IWorker
{
  public DeckEditorViewModel(MTGCardImporter importer, DeckEditorMTGDeck deck = null, Notifier notifier = null, DeckEditorConfirmers confirmers = null)
  {
    Importer = importer;
    Notifier = notifier ?? new();
    Confirmers = confirmers ?? new();

    var cardFilters = new CardFilters();
    var cardSorter = new CardSorter();

    // Cardlists use same sorter and filters
    DeckCardList = CreateCardListViewModel(cardFilters, cardSorter);
    MaybeCardList = CreateCardListViewModel(cardFilters, cardSorter);
    WishCardList = CreateCardListViewModel(cardFilters, cardSorter);
    RemoveCardList = CreateCardListViewModel(cardFilters, cardSorter);

    CommanderCommands = CreateCommanderCommands(CommanderCommands.CommanderType.Commander);
    PartnerCommands = CreateCommanderCommands(CommanderCommands.CommanderType.Partner);

    Deck = deck ?? new();
  }

  public IRepository<MTGCardDeckDTO> Repository { get; init; } = new DeckDTORepository();
  public ReversibleCommandStack UndoStack { get; } = new();
  public DeckEditorConfirmers Confirmers { get; }
  public Notifier Notifier { get; } = new();
  public MTGCardImporter Importer { get; }
  public CardListViewModel DeckCardList { get; }
  public CardListViewModel MaybeCardList { get; }
  public CardListViewModel WishCardList { get; }
  public CardListViewModel RemoveCardList { get; }
  public CommanderCommands CommanderCommands { get; }
  public CommanderCommands PartnerCommands { get; }

  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public MTGCardDeckDTO DTO => DeckEditorMTGDeckToDTOConverter.Convert(Deck);
  public IWorker Worker => this;

  public string Name { get => Deck.Name; set { Deck.Name = value; OnPropertyChanged(nameof(Name)); } }
  public int Size => Deck.DeckCards.Sum(x => x.Count) + (Deck.Commander != null ? 1 : 0) + (Deck.CommanderPartner != null ? 1 : 0);
  public double Price => Deck.DeckCards.Sum(x => x.Info.Price * x.Count) + (Deck.Commander?.Info.Price ?? 0) + (Deck.CommanderPartner?.Info.Price ?? 0);
  public DeckEditorMTGCard Commander
  {
    get => Deck.Commander;
    set
    {
      Deck.Commander = value;
      OnPropertyChanged(nameof(Commander));
      OnPropertyChanged(nameof(Size));
      OnPropertyChanged(nameof(Price));
      OpenEdhrecCommanderWebsiteCommand.NotifyCanExecuteChanged();
      ShowDeckTokensCommand.NotifyCanExecuteChanged();
    }
  }
  public DeckEditorMTGCard Partner
  {
    get => Deck.CommanderPartner;
    set
    {
      Deck.CommanderPartner = value;
      OnPropertyChanged(nameof(Partner));
      OnPropertyChanged(nameof(Size));
      OnPropertyChanged(nameof(Price));
      OpenEdhrecCommanderWebsiteCommand.NotifyCanExecuteChanged();
      ShowDeckTokensCommand.NotifyCanExecuteChanged();
    }
  }

  private DeckEditorMTGDeck Deck
  {
    get => deck;
    set
    {
      if (deck == value) return;

      var oldName = deck != null ? Name : string.Empty;

      deck = value;

      DeckCardList.Cards = deck.DeckCards;
      MaybeCardList.Cards = deck.Maybelist;
      WishCardList.Cards = deck.Wishlist;
      RemoveCardList.Cards = deck.Removelist;

      // Can't invoke changed event for the name if the name was already empty because visual state binding would break.
      if (Name != oldName) OnPropertyChanged(nameof(Name));
      OnPropertyChanged(nameof(Deck));
      OnPropertyChanged(nameof(Size));
      OnPropertyChanged(nameof(Price));
      OnPropertyChanged(nameof(Commander));
      OnPropertyChanged(nameof(Partner));

      SaveDeckCommand.NotifyCanExecuteChanged();
      DeleteDeckCommand.NotifyCanExecuteChanged();
      OpenEdhrecCommanderWebsiteCommand.NotifyCanExecuteChanged();
      ShowDeckTokensCommand.NotifyCanExecuteChanged();
    }
  }

  public IAsyncRelayCommand<ISavable.ConfirmArgs> ConfirmUnsavedChangesCommand => (confirmUnsavedChanges ??= new ConfirmUnsavedChanges(this)).Command;
  public IAsyncRelayCommand NewDeckCommand => (newDeck ??= new NewDeck(this)).Command;
  public IAsyncRelayCommand<string> OpenDeckCommand => (openDeck ??= new OpenDeck(this)).Command;
  public IAsyncRelayCommand SaveDeckCommand => (saveDeck ??= new SaveDeck(this)).Command;
  public IAsyncRelayCommand DeleteDeckCommand => (deleteDeck ??= new DeleteDeck(this)).Command;
  public IRelayCommand UndoCommand => (undo ??= new Undo(this)).Command;
  public IRelayCommand RedoCommand => (redo ??= new Redo(this)).Command;
  public IAsyncRelayCommand OpenEdhrecCommanderWebsiteCommand => (openEdhrecCommanderWebsite ??= new OpenEdhrecCommanderWebsite(this)).Command;
  public IAsyncRelayCommand ShowDeckTokensCommand => (showDeckTokens ??= new ShowDeckTokens(this)).Command;

  private DeckEditorMTGDeck deck;
  private ConfirmUnsavedChanges confirmUnsavedChanges;
  private NewDeck newDeck;
  private OpenDeck openDeck;
  private SaveDeck saveDeck;
  private DeleteDeck deleteDeck;
  private Undo undo;
  private Redo redo;
  private OpenEdhrecCommanderWebsite openEdhrecCommanderWebsite;
  private ShowDeckTokens showDeckTokens;

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

  public void SetDeck(DeckEditorMTGDeck deck)
  {
    if (deck == Deck) return;

    Deck = deck;

    UndoStack.Clear();
    HasUnsavedChanges = false;
  }

  private void OnCommanderChanged(DeckEditorMTGCard card)
  {
    if (Commander == card) return;

    Commander = card;
    HasUnsavedChanges = true;
  }

  private void OnPartnerChanged(DeckEditorMTGCard card)
  {
    if (Partner == card) return;

    Partner = card;
    HasUnsavedChanges = true;
  }

  private CardListViewModel CreateCardListViewModel(CardFilters filters, CardSorter sorter)
  {
    return new(Importer)
    {
      OnChange = () =>
      {
        HasUnsavedChanges = true;
        OnPropertyChanged(nameof(Size));
        OnPropertyChanged(nameof(Price));
        ShowDeckTokensCommand.NotifyCanExecuteChanged();
      },
      UndoStack = UndoStack,
      Worker = this,
      Confirmers = Confirmers.CardListConfirmers,
      Notifier = Notifier,
      CardFilters = filters,
      CardSorter = sorter,
    };
  }

  private CommanderCommands CreateCommanderCommands(CommanderCommands.CommanderType commanderType)
  {
    return new(this, commanderType)
    {
      UndoStack = UndoStack,
      Notifier = Notifier,
      Confirmers = Confirmers.CommanderConfirmers,
      OnChange = commanderType == CommanderCommands.CommanderType.Commander ? OnCommanderChanged : OnPartnerChanged,
      Worker = this,
    };
  }
}