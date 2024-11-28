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
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels;
public partial class DeckEditorViewModel : ObservableObject, ISavable, IWorker
{
  public DeckEditorViewModel(MTGCardImporter importer, DeckEditorMTGDeck? deck = null, Notifier? notifier = null, DeckEditorConfirmers? confirmers = null)
  {
    Commands = new(this);

    Importer = importer;
    Notifier = notifier ?? new();
    Confirmers = confirmers ?? new();
    Deck = deck ?? new();

    var cardFilters = new CardFilters();
    var cardSorter = new CardSorter();

    // Cardlists use the same sorter and filters
    DeckCardList = CreateGroupedCardListViewModel(Deck.DeckCards, cardFilters, cardSorter);
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
  public GroupedCardListViewModel DeckCardList { get; }
  public CardListViewModel MaybeCardList { get; }
  public CardListViewModel WishCardList { get; }
  public CardListViewModel RemoveCardList { get; }
  public CommanderCommands CommanderCommands { get; }
  public CommanderCommands PartnerCommands { get; }

  [ObservableProperty] public partial bool IsBusy { get; set; }
  [ObservableProperty] public partial bool HasUnsavedChanges { get; set; }

  public MTGCardDeckDTO DTO => DeckEditorMTGDeckToDTOConverter.Convert(Deck);
  public IWorker Worker => this;

  public string Name
  {
    get => Deck.Name;
    set
    {
      if (deckName != value)
      {
        deckName = value;
        Deck.Name = value;
        OnPropertyChanged(nameof(Name));
      }
    }
  }
  public int Size => Deck.DeckCards.Sum(x => x.Count) + (Deck.Commander != null ? 1 : 0) + (Deck.CommanderPartner != null ? 1 : 0);
  public double Price => Deck.DeckCards.Sum(x => x.Info.Price * x.Count) + (Deck.Commander?.Info.Price ?? 0) + (Deck.CommanderPartner?.Info.Price ?? 0);
  public DeckEditorMTGCard? Commander
  {
    get => Deck.Commander;
    set
    {
      Deck.Commander = value;
      OnPropertyChanged(nameof(Commander));
    }
  }
  public DeckEditorMTGCard? Partner
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
    get;
    set => SetProperty(ref field, value);
  }
  private DeckEditorViewModelCommands Commands { get; }

  private string deckName = string.Empty;

  public IAsyncRelayCommand<ISavable.ConfirmArgs> ConfirmUnsavedChangesCommand => Commands.ConfirmUnsavedChangesCommand;
  public IAsyncRelayCommand NewDeckCommand => Commands.NewDeckCommand;
  public IAsyncRelayCommand<string> OpenDeckCommand => Commands.OpenDeckCommand;
  public IAsyncRelayCommand SaveDeckCommand => Commands.SaveDeckCommand;
  public IAsyncRelayCommand DeleteDeckCommand => Commands.DeleteDeckCommand;
  public IRelayCommand UndoCommand => Commands.UndoCommand;
  public IRelayCommand RedoCommand => Commands.RedoCommand;
  public IAsyncRelayCommand OpenEdhrecCommanderWebsiteCommand => Commands.OpenEdhrecCommanderWebsiteCommand;
  public IAsyncRelayCommand ShowDeckTokensCommand => Commands.ShowTokensCommand;
  public IRelayCommand OpenDeckTestingWindowCommand => Commands.OpenDeckTestingWindowCommand;
  public IRelayCommand OpenEdhrecSearchWindowCommand => Commands.OpenEdhrecSearchWindowCommand;

  public void SetDeck(DeckEditorMTGDeck deck)
  {
    if (deck == Deck) return;

    Deck = deck;

    UndoStack.Clear();
    HasUnsavedChanges = false;
  }

  private void OnCommanderChanged(DeckEditorMTGCard? card)
  {
    if (Commander == card) return;

    Commander = card;
    HasUnsavedChanges = true;
  }

  private void OnPartnerChanged(DeckEditorMTGCard? card)
  {
    if (Partner == card) return;

    Partner = card;
    HasUnsavedChanges = true;
  }

  private CardListViewModel CreateCardListViewModel(ObservableCollection<DeckEditorMTGCard> cards, CardFilters filters, CardSorter sorter)
  {
    return new(
      importer: Importer,
      confirmers: Confirmers.CardListConfirmers)
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
      Notifier = Notifier,
      CardFilters = filters,
      CardSorter = sorter,
    };
  }

  private GroupedCardListViewModel CreateGroupedCardListViewModel(ObservableCollection<DeckEditorMTGCard> cards, CardFilters filters, CardSorter sorter)
  {
    return new(
      importer: Importer,
      confirmers: Confirmers.CardListConfirmers)
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

  private void DeckEditorViewModel_PropertyChanged(object? _, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(Deck):
        Name = Deck.Name;

        DeckCardList.Cards = Deck.DeckCards;
        MaybeCardList.Cards = Deck.Maybelist;
        WishCardList.Cards = Deck.Wishlist;
        RemoveCardList.Cards = Deck.Removelist;

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