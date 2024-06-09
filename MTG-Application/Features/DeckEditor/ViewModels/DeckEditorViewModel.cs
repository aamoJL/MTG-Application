using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.Services.DeckEditor;
using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.IOService;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

  public IRepository<MTGCardDeckDTO> Repository { get; init; } = new DeckDTORepository();
  public CardFilters CardFilters { get; init; } = new();
  public CardSorter CardSorter { get; init; } = new();
  public DeckEditorConfirmers Confirmers { get; }
  public Notifier Notifier { get; } = new();
  public CardListViewModel DeckCardList { get; }
  public CardListViewModel MaybeCardList { get; }
  public CardListViewModel WishCardList { get; }
  public CardListViewModel RemoveCardList { get; }
  public CommanderViewModel CommanderViewModel { get; }
  public CommanderViewModel PartnerViewModel { get; }

  public string DeckName => Deck.Name;
  public int DeckSize => Deck.DeckSize;
  public double DeckPrice => Deck.DeckPrice;

  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  private DeckEditorMTGDeck Deck
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
      OpenEDHRECWebsiteCommandCommand.NotifyCanExecuteChanged();
      ShowDeckTokensCommand.NotifyCanExecuteChanged();
    }
  }
  private ReversibleCommandStack UndoStack { get; } = new();
  private MTGCardImporter Importer { get; }

  public async Task<bool> ConfirmUnsavedChanges() => await new ConfirmUnsavedChanges(
    saveUnsavedChangesConfirmer: Confirmers.SaveUnsavedChangesConfirmer,
    saveCommand: SaveDeckCommand)
    .Execute(HasUnsavedChanges, DeckName);

  [RelayCommand]
  public async Task NewDeck() => await new NewDeck(
    unsavedConfirmer: ConfirmUnsavedChanges,
    onDeckChanged: (deck) =>
    {
      Deck = deck;
    }).Execute();

  [RelayCommand(CanExecute = nameof(CanExecuteOpenDeckCommand))]
  private async Task OpenDeck(string loadName = null) => await new OpenDeck(
    repository: Repository,
    importer: Importer,
    notifier: Notifier,
    unsavedConfirmer: ConfirmUnsavedChanges,
    loadConfirmer: Confirmers.LoadDeckConfirmer,
    worker: this,
    onDeckChanged: (deck) =>
    {
      Deck = deck;
    }).Execute(loadName);

  [RelayCommand(CanExecute = nameof(CanExecuteSaveDeckCommand))]
  private async Task SaveDeck() => await new SaveDeck(
    repository: Repository,
    notifier: Notifier,
    saveConfirmer: Confirmers.SaveDeckConfirmer,
    overrideConfirmer: Confirmers.OverrideDeckConfirmer,
    worker: this,
    onNameChanged: (name) =>
    {
      Deck.Name = name;
      OnPropertyChanged(nameof(DeckName));
      HasUnsavedChanges = false;
    }).Execute(Deck);

  [RelayCommand(CanExecute = nameof(CanExecuteDeleteDeckCommand))]
  private async Task DeleteDeck() => await new DeleteDeck(
    repository: Repository,
    notifier: Notifier,
    deleteConfirmer: Confirmers.DeleteDeckConfirmer,
    worker: this,
    onDeckChanged: (deck) =>
    {
      Deck = deck;
    }).Execute(Deck);

  [RelayCommand(CanExecute = nameof(CanExecuteUndoCommand))] private void Undo() => UndoStack.Undo();

  [RelayCommand(CanExecute = nameof(CanExecuteRedoCommand))] private void Redo() => UndoStack.Redo();

  /// <summary>
  /// Opens card's Cardmarket page in web browser
  /// </summary>    
  [RelayCommand(CanExecute = nameof(CanExecuteOpenEDHRECWebsiteCommand))]
  private async Task OpenEDHRECWebsiteCommand()
    => await NetworkService.OpenUri(EdhrecImporter.GetCommanderWebsiteUri(Deck.Commander, Deck.CommanderPartner));

  [RelayCommand(CanExecute = nameof(CanExecuteShowDeckTokensCommand))]
  private async Task ShowDeckTokens()
  {
    var stringBuilder = new StringBuilder();

    stringBuilder.AppendJoin(Environment.NewLine, DeckCardList.Cards.Where(c => c.Info.Tokens.Length > 0).Select(
      c => string.Join(Environment.NewLine, c.Info.Tokens.Select(t => string.Join(Environment.NewLine, t.ScryfallId.ToString())))));

    if (Deck.Commander != null)
      stringBuilder.AppendJoin(Environment.NewLine, Deck.Commander.Info.Tokens.Select(t => t.ScryfallId.ToString()));

    if (Deck.CommanderPartner != null)
      stringBuilder.AppendJoin(Environment.NewLine, Deck.CommanderPartner.Info.Tokens.Select(t => t.ScryfallId.ToString()));

    var tokens = (await ((IWorker)this).DoWork(Importer.ImportFromString(stringBuilder.ToString()))).Found
      .DistinctBy(t => t.Info.OracleId); // Filter duplicates out using OracleId

    await Confirmers.ShowTokensConfirmer.Confirm(DeckEditorConfirmers.GetShowTokensConfirmation(tokens));
  }

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

  private bool CanExecuteSaveDeckCommand() => UseCases.SaveDeck.CanExecute(Deck);

  private bool CanExecuteDeleteDeckCommand() => UseCases.DeleteDeck.CanExecute(Deck);

  private bool CanExecuteOpenDeckCommand(string name) => UseCases.OpenDeck.CanExecute(name);

  private bool CanExecuteUndoCommand() => UndoStack.CanUndo;

  private bool CanExecuteRedoCommand() => UndoStack.CanRedo;

  private bool CanExecuteOpenEDHRECWebsiteCommand() => Deck.Commander != null;

  private bool CanExecuteShowDeckTokensCommand() => Deck.Commander != null || Deck.DeckCards.Count != 0;

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
    var vm = new CommanderViewModel(Importer)
    {
      UndoStack = UndoStack,
      Notifier = Notifier,
      Confirmers = Confirmers.CommanderConfirmers,
      OnCardPropertyChange = () =>
      {
        HasUnsavedChanges = true;
        OnPropertyChanged(nameof(DeckPrice));
      },
      Worker = this,
    };

    var changeAction = (DeckEditorMTGCard card) =>
    {
      modelChangeAction?.Invoke(card);
      vm.Card = card;
      HasUnsavedChanges = true;
      OnPropertyChanged(nameof(DeckSize));
      OnPropertyChanged(nameof(DeckPrice));
      OpenEDHRECWebsiteCommandCommand.NotifyCanExecuteChanged();
      ShowDeckTokensCommand.NotifyCanExecuteChanged();
    };

    vm.ReversibleChange = new ReversibleAction<DeckEditorMTGCard>()
    {
      Action = changeAction,
      ReverseAction = changeAction,
    };

    return vm;
  }
}