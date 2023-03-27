using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.Services;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels
{
  public class MTGCardViewModelSource : MTGCardSource<MTGCardViewModel>
  {
    public MTGCardViewModelSource() : base() { }

    protected override MTGCardViewModel GetCardViewModel(MTGCard card) => new(card);
  }

  public class MTGCardCollectionCardViewModelSource : MTGCardSource<MTGCardCollectionCardViewModel>
  {
    public MTGCardCollectionCardViewModelSource() : base() { }

    protected override MTGCardCollectionCardViewModel GetCardViewModel(MTGCard card) => new(card);
  }

  public partial class MTGAPISearch<TSource, IType> : ObservableObject where TSource : MTGCardSource<IType>, new() where IType : MTGCardViewModel
  {
    public MTGAPISearch(ICardAPI<MTGCard> cardAPI)
    {
      this.cardAPI = cardAPI;
    }

    protected readonly ICardAPI<MTGCard> cardAPI;

    [ObservableProperty]
    private IncrementalLoadingCollection<TSource, IType> searchCards = new();
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
      var result = await cardAPI.FetchFromPageUri(cardAPI.GetSearchUri(SearchQuery));
      TotalCardCount = result.TotalCount;
      var source = new TSource()
      {
        Cards = result.Found.ToList(),
        CardAPI = cardAPI,
        NextPage = result.NextPageUri,
      };
      SearchCards = new(source, cardAPI.PageSize);
      IsBusy = false;
    }
  }
}
