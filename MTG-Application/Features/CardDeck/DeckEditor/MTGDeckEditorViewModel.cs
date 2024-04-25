using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.API.CardAPI;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.ViewModels;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;
public partial class MTGDeckEditorViewModel : ViewModelBase, ISavable, IWorker
{
  [ObservableProperty] private MTGCardDeck deck = new();
  [ObservableProperty] private bool isBusy;
  [ObservableProperty] private bool hasUnsavedChanges;

  public MTGDeckEditorViewModelConfirmer Confirmer { get; init; } = new();
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
        case ConfirmationResult.Success: Deck = loadResult.Deck; break; // TODO: Success
        case ConfirmationResult.Failure: break; // TODO: Error
        case ConfirmationResult.Cancel:
        default: break;
      }
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteSaveDeckCommand))]
  private async Task SaveDeck(string saveName = null)
  {
    if (!CanExecuteDeleteDeckCommand()) return;

    switch (await SaveDeckUseCase.Execute(new(Deck, saveName)))
    {
      case ConfirmationResult.Success: break; // TODO: Success
      case ConfirmationResult.Failure: return; // TODO: Error
      case ConfirmationResult.Cancel:
      default:
        break;
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

    if (await ConfirmUnsavedChanges())
    {
      switch (await DeleteDeckUseCase.Execute(Deck))
      {
        case ConfirmationResult.Success: Deck = new(); return;
        case ConfirmationResult.Failure: return; // TODO: Error
        case ConfirmationResult.Cancel:
        default: return;
      }
    }
  }

  public async Task<bool> ConfirmUnsavedChanges()
  {
    if (!HasUnsavedChanges) return true;

    return await SaveUnsavedChangesUseCase.Execute(new(Deck)) switch
    {
      ConfirmationResult.Success => true,
      ConfirmationResult.Failure => false, // TODO: Error
      ConfirmationResult.Cancel => false,
      _ => false,
    };
  }

  public async Task<T> DoWork<T>(Task<T> task)
  {
    IsBusy = true;
    var result = await task;
    IsBusy = false;
    return result;
  }

  private bool CanExecuteSaveDeckCommand() => Deck.DeckCards.Count > 0;

  private bool CanExecuteDeleteDeckCommand() => !string.IsNullOrEmpty(Deck.Name);
}

public partial class MTGDeckEditorViewModel
{
  private SaveUnsavedChangesUseCase SaveUnsavedChangesUseCase => new(Repository)
  {
    UnsavedChangesConfirmation = Confirmer.SaveUnsavedChanges,
    SaveConfirmation = Confirmer.SaveDeck,
    OverrideConfirmation = Confirmer.OverrideDeck,
    Worker = this
  };

  private LoadDeckUseCase LoadDeckUseCase => new(Repository, CardAPI)
  {
    LoadConfirmation = Confirmer.LoadDeck,
    Worker = this
  };

  private SaveDeckUseCase SaveDeckUseCase => new(Repository)
  {
    SaveConfirmation = Confirmer.SaveDeck,
    OverrideConfirmation = Confirmer.OverrideDeck,
    Worker = this
  };

  private DeleteDeckUseCase DeleteDeckUseCase => new(Repository)
  {
    DeleteConfirmation = Confirmer.DeleteDeckUseCase,
    Worker = this
  };
}