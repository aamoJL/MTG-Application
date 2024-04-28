using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;
public partial class DeckSelectorViewModel : ViewModelBase
{
  public DeckSelectorViewModel(IRepository<MTGCardDeckDTO> repository, ICardAPI<MTGCard> cardAPI)
  {
    Repository = repository;
    CardAPI = cardAPI;
  }

  public ObservableCollection<DeckSelectionListItem> DeckItems { get; } = new();
  public IRepository<MTGCardDeckDTO> Repository { get; }
  public ICardAPI<MTGCard> CardAPI { get; }

  [ObservableProperty] private bool isBusy;

  [RelayCommand]
  private async Task LoadDecks()
  {
    IsBusy = true;
    var deckNameImageTuples = await new GetDeckNamesAndImageUris(Repository, CardAPI).Execute();

    DeckItems.Clear();

    foreach (var (Name, ImageUri) in deckNameImageTuples) DeckItems.Add(new(Name, ImageUri));

    IsBusy = false;
  }
}
