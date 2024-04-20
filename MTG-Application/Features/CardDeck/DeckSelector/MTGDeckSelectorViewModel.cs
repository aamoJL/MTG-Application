using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardDeck;
public partial class MTGDeckSelectorViewModel : ViewModelBase
{
  public ObservableCollection<MTGDeckSelectionListItem> DeckItems { get; } = new();

  [ObservableProperty] private bool isBusy;

  [RelayCommand]
  private async Task LoadDecks()
  {
    IsBusy = true;
    var deckNameImageTuples = await new GetMTGDeckNamesAndImageUris(contextFactory: new(), App.MTGCardAPI).Execute();

    DeckItems.Clear();

    foreach (var (Name, ImageUri) in deckNameImageTuples) DeckItems.Add(new(Name, ImageUri));

    IsBusy = false;
  }
}
