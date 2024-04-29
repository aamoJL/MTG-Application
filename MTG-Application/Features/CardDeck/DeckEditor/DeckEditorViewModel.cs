using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.DeckRepository;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplication.Features.CardDeck;
public partial class DeckEditorViewModel : ViewModelBase, ISavable, IWorker
{
  [ObservableProperty] private MTGCardDeck deck = new();
  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public DeckEditorConfirmers Confirmers { get; init; } = new();
  public DeckEditorNotifier Notifier { get; init; } = new();
  public IRepository<MTGCardDeckDTO> Repository { get; init; } = new DeckDTORepository();
  public ICardAPI<MTGCard> CardAPI { get; init; } = App.MTGCardAPI;

  [RelayCommand]
  private async Task NewDeck()
  {
    if (await ConfirmUnsavedChanges())
      Deck = new();
  }

  [RelayCommand]
  private async Task OpenDeck(string loadName = null)
  {
    if (loadName == string.Empty) return;

    if (await ConfirmUnsavedChanges())
    {
      var loadResult = await LoadDeckUseCase.Execute(loadName);

      switch (loadResult.ConfirmResult)
      {
        case ConfirmationResult.Yes:
          Deck = loadResult.Deck;
          Notifier.Notify(Notifier.Notifications.LoadSuccessNotification); break;
        case ConfirmationResult.Failure: Notifier.Notify(Notifier.Notifications.LoadErrorNotification); break;
        default: break;
      }
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteSaveDeckCommand))]
  private async Task SaveDeck()
  {
    if (!CanExecuteDeleteDeckCommand()) return;

    switch (await SaveDeckUseCase.Execute(Deck))
    {
      case ConfirmationResult.Yes: Notifier.Notify(Notifier.Notifications.SaveSuccessNotification); break;
      case ConfirmationResult.Failure: Notifier.Notify(Notifier.Notifications.SaveErrorNotification); return;
      default: break;
    }
  }

  [RelayCommand] private void RemoveDeckCard(MTGCard card) => Deck.DeckCards.Remove(card);

  [RelayCommand]
  private void ImportDeckCards(string importText)
  {
    // TODO: Import
  }

  [RelayCommand(CanExecute = nameof(CanExecuteDeleteDeckCommand))]
  private async Task DeleteDeck()
  {
    if (!CanExecuteDeleteDeckCommand()) return;

    switch (await DeleteDeckUseCase.Execute(Deck))
    {
      case ConfirmationResult.Yes:
        Deck = new();
        Notifier.Notify(Notifier.Notifications.DeleteSuccessNotification); break;
      case ConfirmationResult.Failure: Notifier.Notify(Notifier.Notifications.DeleteErrorNotification); break;
      default: return;
    }
  }

  public async Task<bool> ConfirmUnsavedChanges()
  {
    if (!HasUnsavedChanges) return true;

    switch (await SaveUnsavedChangesUseCase.Execute(Deck))
    {
      case ConfirmationResult.Yes: Notifier.Notify(Notifier.Notifications.SaveSuccessNotification); return true;
      case ConfirmationResult.No: return true;
      case ConfirmationResult.Failure: Notifier.Notify(Notifier.Notifications.SaveErrorNotification); return false;
      default: return false;
    }
  }

  private bool CanExecuteSaveDeckCommand() => Deck.DeckCards.Count > 0;

  private bool CanExecuteDeleteDeckCommand() => !string.IsNullOrEmpty(Deck.Name);
}

public partial class DeckEditorViewModel
{
  private SaveUnsavedChanges SaveUnsavedChangesUseCase => new(Repository)
  {
    UnsavedChangesConfirmation = Confirmers.SaveUnsavedChanges,
    SaveConfirmation = Confirmers.SaveDeck,
    OverrideConfirmation = Confirmers.OverrideDeck,
    Worker = this
  };

  private LoadDeck LoadDeckUseCase => new(Repository, CardAPI)
  {
    LoadConfirmation = Confirmers.LoadDeck,
    Worker = this
  };

  private SaveDeck SaveDeckUseCase => new(Repository)
  {
    SaveConfirmation = Confirmers.SaveDeck,
    OverrideConfirmation = Confirmers.OverrideDeck,
    Worker = this
  };

  private DeleteDeck DeleteDeckUseCase => new(Repository)
  {
    DeleteConfirmation = Confirmers.DeleteDeckUseCase,
    Worker = this
  };
}