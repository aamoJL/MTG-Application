using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.DeckRepository;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.IOService;
using MTGApplication.General.Services.NotificationService;
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
  private MTGCardDeck deck = new();

  public DeckEditorViewModel(ICardAPI<MTGCard> cardAPI, MTGCardDeck deck = null, Notifier notifier = null, DeckEditorConfirmers confirmers = null)
  {
    CardAPI = cardAPI;
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
  private ICardAPI<MTGCard> CardAPI { get; }

  public DeckEditorConfirmers Confirmers { get; }
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
  public double DeckPrice => Deck.DeckPrice;

  public async Task<bool> ConfirmUnsavedChanges()
  {
    if (!HasUnsavedChanges || !SaveDeckCommand.CanExecute(null)) return true;

    switch (await Confirmers.SaveUnsavedChangesConfirmer
      .Confirm(DeckEditorConfirmers.GetSaveUnsavedChangesConfirmation(DeckName)))
    {
      case ConfirmationResult.Yes: await SaveDeck(); return !HasUnsavedChanges;
      case ConfirmationResult.No: return true;
      default: return false;
    };
  }

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

    loadName ??= await Confirmers.LoadDeckConfirmer
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
    var saveName = await Confirmers.SaveDeckConfirmer.Confirm(
      DeckEditorConfirmers.GetSaveDeckConfirmation(oldName));

    if (string.IsNullOrEmpty(saveName))
      return;

    // Override confirmation
    if (saveName != oldName && await new DeckExists(Repository).Execute(saveName))
    {
      switch (await Confirmers.OverrideDeckConfirmer.Confirm(DeckEditorConfirmers.GetOverrideDeckConfirmation(saveName)))
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

    var deleteConfirmationResult = await Confirmers.DeleteDeckConfirmer.Confirm(
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
  [RelayCommand(CanExecute = nameof(CanExecuteOpenEDHRECWebsiteCommand))]
  private async Task OpenEDHRECWebsiteCommand()
    => await NetworkService.OpenUri(EdhrecAPI.GetCommanderWebsiteUri(Deck.Commander, Deck.CommanderPartner));

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

    var tokens = (await ((IWorker)this).DoWork(CardAPI.FetchFromString(stringBuilder.ToString()))).Found
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

  private bool CanExecuteSaveDeckCommand() => Deck.DeckCards.Count > 0;

  private bool CanExecuteDeleteDeckCommand() => !string.IsNullOrEmpty(Deck.Name);

  private bool CanExecuteOpenDeckCommand(string name) => name != string.Empty;

  private bool CanExecuteUndoCommand() => UndoStack.CanUndo;

  private bool CanExecuteRedoCommand() => UndoStack.CanRedo;

  private bool CanExecuteOpenEDHRECWebsiteCommand() => Deck.Commander != null;

  private bool CanExecuteShowDeckTokensCommand() => Deck.Commander != null || Deck.DeckCards.Count != 0;

  private CardListViewModel CreateCardListViewModel(ObservableCollection<MTGCard> cards)
  {
    return new(CardAPI)
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

  private CommanderViewModel CreateCommanderViewModel(Action<MTGCard> modelChangeAction)
  {
    var vm = new CommanderViewModel(CardAPI)
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

    var changeAction = (MTGCard card) =>
    {
      modelChangeAction?.Invoke(card);
      vm.Card = card;
      HasUnsavedChanges = true;
      OnPropertyChanged(nameof(DeckSize));
      OnPropertyChanged(nameof(DeckPrice));
      OpenEDHRECWebsiteCommandCommand.NotifyCanExecuteChanged();
      ShowDeckTokensCommand.NotifyCanExecuteChanged();
    };

    vm.ReversibleChange = new ReversibleAction<MTGCard>()
    {
      Action = changeAction,
      ReverseAction = changeAction,
    };

    return vm;
  }
}