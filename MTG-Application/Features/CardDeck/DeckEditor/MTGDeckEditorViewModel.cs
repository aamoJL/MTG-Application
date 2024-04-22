using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplication.Models;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;

public partial class MTGDeckEditorViewModel : ViewModelBase
{
  [ObservableProperty] private MTGCardDeck deck = new();
  [ObservableProperty] private bool isBusy;

  [RelayCommand]
  private void NewDeck() => Deck = new();

  [RelayCommand(CanExecute = nameof(CanExecuteGetDeckCommand))]
  private async Task LoadDeck(string name)
  {
    IsBusy = true;

    var loadedDeck = await new GetDeckUseCase(
      name: name,
      repository: new DeckDTORepository(new()),
      cardAPI: App.MTGCardAPI)
    .Execute();

    if (loadedDeck != null) Deck = loadedDeck;
    else
    {
      // TODO: error
    }

    IsBusy = false;
  }

  [RelayCommand(CanExecute = nameof(CanExecuteSaveDeckCommand))]
  private void SaveDeck(string name)
  {
    IsBusy = true;

    // TODO: save

    IsBusy = false;
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
}

public partial class MTGDeckEditorViewModel
{
  private bool CanExecuteSaveDeckCommand(string name) => !string.IsNullOrEmpty(name);

  private bool CanExecuteGetDeckCommand(string name) => !string.IsNullOrEmpty(name);

  private bool CanExecuteImportDeckCardsCommand(string importText) => !string.IsNullOrEmpty(importText);

  private bool CanExecuteDeleteDeckCommand() => !string.IsNullOrEmpty(Deck.Name);
}
