using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.DeckEditorViewModelCommands;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor;
public partial class DeckEditorViewModel : ViewModelBase, ISavable, IWorker
{
  public DeckEditorViewModel(MTGCardImporter importer, DeckEditorMTGDeck deck = null, Notifier notifier = null, DeckEditorConfirmers confirmers = null)
  {
    Importer = importer;
    Notifier = notifier ?? new();
    Confirmers = confirmers ?? new();
    Deck = deck ?? new();

    var cardFilters = new CardFilters();
    var cardSorter = new CardSorter();

    DeckCardList = CreateCardListViewModel(Deck.DeckCards, cardFilters, cardSorter);
    MaybeCardList = CreateCardListViewModel(Deck.Maybelist, cardFilters, cardSorter);
    WishCardList = CreateCardListViewModel(Deck.Wishlist, cardFilters, cardSorter);
    RemoveCardList = CreateCardListViewModel(Deck.Removelist, cardFilters, cardSorter);

    CommanderViewModel = CreateCommanderViewModel(modelChangeAction: (card) => { Deck.Commander = card; });
    PartnerViewModel = CreateCommanderViewModel(modelChangeAction: (card) => { Deck.CommanderPartner = card; });
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
  public CommanderViewModel CommanderViewModel { get; }
  public CommanderViewModel PartnerViewModel { get; }

  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public string Name { get => Deck.Name; set { Deck.Name = value; OnPropertyChanged(nameof(Name)); } }
  public int Size => Deck.DeckCards.Sum(x => x.Count) + (Deck.Commander != null ? 1 : 0) + (Deck.CommanderPartner != null ? 1 : 0);
  public double Price => Deck.DeckCards.Sum(x => x.Info.Price * x.Count) + (Deck.Commander?.Info.Price ?? 0) + (Deck.CommanderPartner?.Info.Price ?? 0);
  public DeckEditorMTGCard Commander => Deck.Commander;
  public DeckEditorMTGCard Partner => Deck.CommanderPartner;
  public ReadOnlyCollection<DeckEditorMTGCard> DeckCards => Deck.DeckCards.AsReadOnly();

  public MTGCardDeckDTO DTO => DeckEditorMTGDeckToDTOConverter.Convert(Deck);
  public IWorker Worker => this;

  public IAsyncRelayCommand<ISavable.ConfirmArgs> ConfirmUnsavedChangesCommand => confirmUnsavedChanges?.Command ?? (confirmUnsavedChanges = new ConfirmUnsavedChanges(this)).Command;
  public IAsyncRelayCommand NewDeckCommand => newDeck?.Command ?? (newDeck = new NewDeck(this)).Command;
  public IAsyncRelayCommand<string> OpenDeckCommand => openDeck?.Command ?? (openDeck = new OpenDeck(this)).Command;
  public IAsyncRelayCommand SaveDeckCommand => saveDeck?.Command ?? (saveDeck = new SaveDeck(this)).Command;
  public IAsyncRelayCommand DeleteDeckCommand => deleteDeck?.Command ?? (deleteDeck = new DeleteDeck(this)).Command;
  public IRelayCommand UndoCommand => undo?.Command ?? (undo = new Undo(this)).Command;
  public IRelayCommand RedoCommand => redo?.Command ?? (redo = new Redo(this)).Command;
  public IAsyncRelayCommand OpenEdhrecCommanderWebsiteCommand => openEdhrecCommanderWebsite?.Command ?? (openEdhrecCommanderWebsite = new OpenEdhrecCommanderWebsite(this)).Command;
  public IAsyncRelayCommand ShowDeckTokensCommand => showDeckTokens?.Command ?? (showDeckTokens = new ShowDeckTokens(this)).Command;

  private DeckEditorMTGDeck Deck { get; set; }
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

    var oldName = Name;

    Deck = deck;

    DeckCardList.Cards = deck.DeckCards;
    MaybeCardList.Cards = deck.Maybelist;
    WishCardList.Cards = deck.Wishlist;
    RemoveCardList.Cards = deck.Removelist;

    CommanderViewModel.Card = deck.Commander;
    PartnerViewModel.Card = deck.CommanderPartner;

    UndoStack.Clear();
    HasUnsavedChanges = false;

    OnPropertyChanged(nameof(Deck));
    // Can't invoke changed event for the name if the name was already empty because visual state binding would break.
    if (Name != oldName) OnPropertyChanged(nameof(Name));
    OnPropertyChanged(nameof(Size));
    OnPropertyChanged(nameof(Price));
    SaveDeckCommand.NotifyCanExecuteChanged();
    DeleteDeckCommand.NotifyCanExecuteChanged();
    OpenEdhrecCommanderWebsiteCommand.NotifyCanExecuteChanged();
    ShowDeckTokensCommand.NotifyCanExecuteChanged();
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
        OnPropertyChanged(nameof(Size));
        OnPropertyChanged(nameof(Price));
        OpenEdhrecCommanderWebsiteCommand.NotifyCanExecuteChanged();
        ShowDeckTokensCommand.NotifyCanExecuteChanged();
      },
      Worker = this,
    };

    return commanderVM;
  }
}