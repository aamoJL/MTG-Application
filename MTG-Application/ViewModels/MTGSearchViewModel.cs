using CommunityToolkit.Common.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels
{
  public partial class MTGSearchViewModel : ObservableObject
  {
    public class MTGCardSource : IIncrementalSource<MTGCardViewModel>
    {
      private readonly List<MTGCard> cards = new();
      private readonly ICardAPI<MTGCard> cardAPI;
      private string nextPage;

      public MTGCardSource() { }
      public MTGCardSource(MTGCard[] cardArray, string nextPage, ICardAPI<MTGCard> cardAPI)
      {
        cards.AddRange(cardArray);
        this.nextPage = nextPage;
        this.cardAPI = cardAPI;
      }

      public async Task<IEnumerable<MTGCardViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
      {
        if (!string.IsNullOrEmpty(nextPage))
        {
          // Load next page
          (var newCards, nextPage, _) = await cardAPI.FetchCardsFromPage(nextPage);
          foreach (var card in newCards)
          {
            cards.Add(card);
          }
        }
        return await Task.Run(() => (from p in cards select p).Skip(pageIndex * pageSize).Take(pageSize).Select(x => new MTGCardViewModel(x))); ;
      }
    }

    public MTGSearchViewModel(ICardAPI<MTGCard> cardAPI)
    {
      this.cardAPI = cardAPI;
    }

    private readonly ICardAPI<MTGCard> cardAPI;

    [ObservableProperty]
    private IncrementalLoadingCollection<MTGCardSource, MTGCardViewModel> searchCards = new();
    [ObservableProperty]
    private int totalCardCount;
    [ObservableProperty]
    private string searchQuery;
    [ObservableProperty]
    private bool isBusy;

    /// <summary>
    /// Fetches cards from API using selected query, and replaces current cards with the fetched cards
    /// </summary>
    [RelayCommand]
    public async Task SearchSubmit()
    {
      IsBusy = true;
      (var cards, var nextPage, TotalCardCount) = await cardAPI.FetchCardsFromPage(cardAPI.GetSearchUri(SearchQuery));
      SearchCards = new(new MTGCardSource(cards, nextPage, cardAPI), cardAPI.PageSize);
      IsBusy = false;
    }
  }
}
