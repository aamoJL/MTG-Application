using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.API.CardAPI;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.General.ViewModels;
using MTGApplication.Interfaces;
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
  public IRepository<MTGCardDeckDTO> Repository { get; init; } = new DeckDTORepository(new());
  public ICardAPI<MTGCard> CardAPI { get; init; } = App.MTGCardAPI;

  [RelayCommand]
  private async Task NewDeck()
  {
    if (HasUnsavedChanges)
    {
      if (await Confirmer.SaveUnsavedChanges.Confirm(
        title: "Save unsaved changes?",
        message: $"{(string.IsNullOrEmpty(Deck.Name) ? "Unnamed deck" : $"'{Deck.Name}'")} has unsaved changes. Would you like to save the deck?"
        ) is true)
      {
        if (SaveDeckCommand.CanExecute(Deck)) SaveDeckCommand.Execute(Deck);
      }
      else return; // Canceled
    }

    Deck = new();
  }

  [RelayCommand]
  private async Task OpenDeck(string loadName = null)
  {
    if (loadName == string.Empty) return;

    if (HasUnsavedChanges)
    {
      return;
    }

    var loadTask = new LoadDeckUseCase(Repository, CardAPI)
    {
      LoadConfirmation = Confirmer.LoadDeck,
      Worker = this
    };

    switch (await loadTask.Execute(loadName))
    {
      case MTGCardDeck deck: Deck = deck; break; // Success
      default: break; // Error
    }
  }

  [RelayCommand(CanExecute = nameof(CanExecuteSaveDeckCommand))]
  private async Task SaveDeck(string saveName = null)
  {
    var saveTask = new SaveDeckUseCase(Repository)
    {
      SaveConfirmation = Confirmer.SaveDeck,
      OverrideConfirmation = Confirmer.OverrideDeck,
      Worker = this
    };

    switch (await saveTask.Execute(new(Deck, saveName)))
    {
      case true: return; // Success
      case false: return; // Error
      case null: return; // Cancel
    }
  }

  [RelayCommand] private void RemoveDeckCard(MTGCard card) => Deck.DeckCards.Remove(card);

  [RelayCommand]
  private void ImportDeckCards(string importText)
  {
    // TODO: Import
  }

  [RelayCommand(CanExecute = nameof(CanExecuteDeleteDeckCommand))]
  private void DeleteDeck()
  {
    IsBusy = true;
    // TODO: delete deck
    var deleted = true;
    if (deleted) Deck = new();
    IsBusy = false;
  }

  public async Task<bool> SaveUnsavedChanges() => await Task.FromResult(true);

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