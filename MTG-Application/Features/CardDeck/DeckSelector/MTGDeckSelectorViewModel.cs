using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.API.CardAPI;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.ViewModels;
using MTGApplication.Models;
using MTGApplication.Models.DTOs;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;
public partial class MTGDeckSelectorViewModel : ViewModelBase
{
  public MTGDeckSelectorViewModel(IRepository<MTGCardDeckDTO> repository, ICardAPI<MTGCard> cardAPI)
  {
    Repository = repository;
    CardAPI = cardAPI;
  }

  public ObservableCollection<MTGDeckSelectionListItem> DeckItems { get; } = new();
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
