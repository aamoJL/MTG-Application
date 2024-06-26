using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.Editor.Services.Converters;
using MTGApplication.Features.DeckEditor.Editor.UseCases;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.Editor.UseCases.DeckEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels;
public partial class DeckEditorViewModel : ViewModelBase, ISavable, IWorker
{
  // TODO: DeckViewModel like CardCollectionPage's CardCollectionViewModel
  public DeckEditorViewModel(MTGCardImporter importer, DeckEditorMTGDeck deck = null, Notifier notifier = null, DeckEditorConfirmers confirmers = null)
  {
    Importer = importer;
    Notifier = notifier ?? new();
    Confirmers = confirmers ?? new();
    Deck = deck ?? new();

    var cardFilters = new CardFilters();
    var cardSorter = new CardSorter();

    // Cardlists use same sorter and filters
    DeckCardList = CreateCardListViewModel(Deck.DeckCards, cardFilters, cardSorter);
    MaybeCardList = CreateCardListViewModel(Deck.Maybelist, cardFilters, cardSorter);
    WishCardList = CreateCardListViewModel(Deck.Wishlist, cardFilters, cardSorter);
    RemoveCardList = CreateCardListViewModel(Deck.Removelist, cardFilters, cardSorter);

    CommanderCommands = CreateCommanderCommands(CommanderCommands.CommanderType.Commander);
    PartnerCommands = CreateCommanderCommands(CommanderCommands.CommanderType.Partner);

    PropertyChanged += DeckEditorViewModel_PropertyChanged;
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

  public string Name { get => Deck.Name; set { if (deckName != value) { deckName = value; Deck.Name = value; OnPropertyChanged(nameof(Name)); } } }
  public int Size => Deck.DeckCards.Sum(x => x.Count) + (Deck.Commander != null ? 1 : 0) + (Deck.CommanderPartner != null ? 1 : 0);
  public double Price => Deck.DeckCards.Sum(x => x.Info.Price * x.Count) + (Deck.Commander?.Info.Price ?? 0) + (Deck.CommanderPartner?.Info.Price ?? 0);
  public DeckEditorMTGCard Commander
  {
    get => Deck.Commander;
    set
    {
      Deck.Commander = value;
      OnPropertyChanged(nameof(Commander));

    }
  }
  public DeckEditorMTGCard Partner
  {
    get => Deck.CommanderPartner;
    set
    {
      Deck.CommanderPartner = value;
      OnPropertyChanged(nameof(Partner));
    }
  }

  private DeckEditorMTGDeck Deck
  {
    get => deck;
    set => SetProperty(ref deck, value);
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
  public IRelayCommand OpenDeckTestingWindowCommand => (openDeckTestingWindow ??= new OpenDeckTestingWindow(this)).Command;
  public IRelayCommand OpenEdhrecSearchWindowCommand => (openEdhrecSearchWindow ??= new OpenEdhrecSearchWindow(this)).Command;

  private string deckName = string.Empty;
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
  private OpenDeckTestingWindow openDeckTestingWindow;
  private OpenEdhrecSearchWindow openEdhrecSearchWindow;

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

  private CardListViewModel CreateCardListViewModel(ObservableCollection<DeckEditorMTGCard> cards, CardFilters filters, CardSorter sorter)
  {
    return new(Importer)
    {
      Cards = cards,
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

  private void DeckEditorViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(Deck):
        Name = Deck.Name;

        DeckCardList.Cards = deck.DeckCards;
        MaybeCardList.Cards = deck.Maybelist;
        WishCardList.Cards = deck.Wishlist;
        RemoveCardList.Cards = deck.Removelist;

        OnPropertyChanged(nameof(Size));
        OnPropertyChanged(nameof(Price));
        OnPropertyChanged(nameof(Commander));
        OnPropertyChanged(nameof(Partner));
        SaveDeckCommand.NotifyCanExecuteChanged();
        DeleteDeckCommand.NotifyCanExecuteChanged(); break;
      case nameof(Size):
        ShowDeckTokensCommand.NotifyCanExecuteChanged();
        OpenDeckTestingWindowCommand.NotifyCanExecuteChanged(); break;
      case nameof(Commander):
      case nameof(Partner):
        OnPropertyChanged(nameof(Size));
        OnPropertyChanged(nameof(Price));
        OpenEdhrecCommanderWebsiteCommand.NotifyCanExecuteChanged();
        OpenEdhrecSearchWindowCommand.NotifyCanExecuteChanged(); break;
    }
  }
}