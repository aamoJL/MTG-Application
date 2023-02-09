using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels
{
  public partial class DeckBuilderViewModel
  {
    public partial class MTGSearch : ObservableObject
    {
      public ObservableCollection<MTGCardViewModel> SearchCards { get; set; } = new();

      public string SearchQuery { get; set; }
      [ObservableProperty]
      private bool isBusy;

      /// <summary>
      /// Fetches cards from API using selected query, and replaces current cards with the fetched cards
      /// </summary>
      public async Task Search()
      {
        IsBusy = true;
        SearchCards.Clear();
        var cards = await Task.Run(() => App.CardAPI.FetchCards(SearchQuery));
        foreach (var item in cards)
        {
          SearchCards.Add(new(item));
        }
        IsBusy = false;
      }
    }
  }
}
