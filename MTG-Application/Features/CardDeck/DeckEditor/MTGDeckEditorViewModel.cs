using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;

public partial class MTGDeckEditorViewModel : ViewModelBase, ISavable
{
  public bool HasUnsavedChanges { get; set; }

  [ObservableProperty] private MTGCardDeck deck = new();

  public Task<bool> SaveUnsavedChanges() => Task.FromResult(true);

  [RelayCommand]
  public async Task OpenDeck(string name)
  {
    Deck = await new GetDeckOrDefaultUseCase(
      name: name,
      repository: new DeckDTORepository(new()),
      cardAPI: App.MTGCardAPI)
    .Execute();
  }

  [RelayCommand] public void RemoveDeckCard(MTGCard card) => Deck.DeckCards.Remove(card);
}
