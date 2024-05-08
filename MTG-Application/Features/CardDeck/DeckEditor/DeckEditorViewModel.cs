using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.DeckRepository;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplication.Features.CardDeck;
public partial class DeckEditorViewModel : ViewModelBase, ISavable, IWorker
{
  private MTGCardDeck deck = new();

  public DeckEditorViewModel()
  {
    DeckCards = new() { OnChange = OnDeckCardsChanged };
  }

  public DeckEditorViewModel(MTGCardDeck deck) : this() => Deck = deck;

  private MTGCardDeck Deck
  {
    get => deck;
    set
    {
      deck = value;

      DeckCards.Cards = deck.DeckCards;
      HasUnsavedChanges = true;

      OnPropertyChanged(nameof(DeckSize));
      OnPropertyChanged(nameof(DeckName));
      OnPropertyChanged(nameof(Commander));
      OnPropertyChanged(nameof(Partner));
      SaveDeckCommand.NotifyCanExecuteChanged();
      DeleteDeckCommand.NotifyCanExecuteChanged();
    }
  }

  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public CardFilters CardFilters { get; } = new();
  public CardSorter CardSorter { get; } = new();
  public IRepository<MTGCardDeckDTO> Repository { get; init; } = new DeckDTORepository();
  public ICardAPI<MTGCard> CardAPI { get; init; } = App.MTGCardAPI;
  public DeckEditorConfirmers Confirmers { get; init; } = new();
  public DeckEditorNotifier Notifier { get; init; } = new();

  public CardListViewModel DeckCards { get; }
  
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
  public string DeckName
  {
    get => Deck.Name;
    set
    {
      Deck.Name = value;
      OnPropertyChanged(nameof(DeckName));
    }
  }
  public int DeckSize => Deck.DeckSize;

  public async Task<bool> ConfirmUnsavedChanges()
  {
    if (!HasUnsavedChanges) return true;

    var result = await new SaveUnsavedChanges(Repository)
    {
      UnsavedChangesConfirmation = Confirmers.SaveUnsavedChanges,
      SaveConfirmation = Confirmers.SaveDeck,
      OverrideConfirmation = Confirmers.OverrideDeck,
      Worker = this
    }.Execute(Deck);

    switch (result)
    {
      case ConfirmationResult.Yes: SendNotification(Notifier.Notifications.SaveSuccessNotification); return true;
      case ConfirmationResult.No: return true;
      case ConfirmationResult.Failure: SendNotification(Notifier.Notifications.SaveErrorNotification); return false;
      default: return false;
    }
  }

  private void SendNotification(NotificationService.Notification notification)
    => Notifier.Notify(notification);

  private void OnDeckCardsChanged()
  {
    HasUnsavedChanges = true;
    OnPropertyChanged(nameof(DeckSize));
  }
}

public partial class DeckEditorViewModel
{
  [RelayCommand]
  private async Task NewDeck()
  {
    if (await ConfirmUnsavedChanges())
    {
      Deck = new();
      HasUnsavedChanges = false;
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteOpenDeckCommand))]
  private async Task OpenDeck(string loadName = null)
  {
    if (!OpenDeckCommand.CanExecute(loadName)) return;

    if (await ConfirmUnsavedChanges())
    {
      var loadResult = await new LoadDeck(Repository, CardAPI)
      {
        LoadConfirmation = Confirmers.LoadDeck,
        Worker = this
      }.Execute(loadName);

      switch (loadResult.ConfirmResult)
      {
        case ConfirmationResult.Yes:
          Deck = loadResult.Deck;
          HasUnsavedChanges = false;
          SendNotification(Notifier.Notifications.LoadSuccessNotification); break;
        case ConfirmationResult.Failure: SendNotification(Notifier.Notifications.LoadErrorNotification); break;
        default: break;
      }
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteSaveDeckCommand))]
  private async Task SaveDeck()
  {
    if (!SaveDeckCommand.CanExecute(null)) return;

    var result = await new SaveDeck(Repository)
    {
      SaveConfirmation = Confirmers.SaveDeck,
      OverrideConfirmation = Confirmers.OverrideDeck,
      Worker = this
    }.Execute(Deck);

    switch (result)
    {
      case ConfirmationResult.Yes:
        SendNotification(Notifier.Notifications.SaveSuccessNotification);
        OnPropertyChanged(nameof(DeckName));
        HasUnsavedChanges = false;
        break;
      case ConfirmationResult.Failure: SendNotification(Notifier.Notifications.SaveErrorNotification); return;
      default: break;
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteDeleteDeckCommand))]
  private async Task DeleteDeck()
  {
    if (!DeleteDeckCommand.CanExecute(null)) return;

    var result = await new DeleteDeck(Repository)
    {
      DeleteConfirmation = Confirmers.DeleteDeck,
      Worker = this
    }.Execute(Deck);

    switch (result)
    {
      case ConfirmationResult.Yes:
        Deck = new();
        HasUnsavedChanges = false;
        SendNotification(Notifier.Notifications.DeleteSuccessNotification); break;
      case ConfirmationResult.Failure: SendNotification(Notifier.Notifications.DeleteErrorNotification); break;
      default: return;
    }
  }

  private bool CanExecuteSaveDeckCommand() => Deck.DeckCards.Count > 0;

  private bool CanExecuteDeleteDeckCommand() => !string.IsNullOrEmpty(Deck.Name);

  private bool CanExecuteOpenDeckCommand(string name) => name != string.Empty;
}