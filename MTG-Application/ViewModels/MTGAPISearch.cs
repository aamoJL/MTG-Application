using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels;

/// <summary>
/// View model for MTG card API search
/// </summary>
public partial class MTGAPISearch<TSource, IType> : ObservableObject where TSource : MTGCardSource<IType>, new() where IType : MTGCardViewModel
{
  public MTGAPISearch(ICardAPI<MTGCard> cardAPI) => this.CardAPI = cardAPI;

  public ICardAPI<MTGCard> CardAPI { get; }

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
    var result = await CardAPI.FetchFromUri(CardAPI.GetSearchUri(SearchQuery));
    TotalCardCount = result.TotalCount;

    var source = new TSource()
    {
      Cards = result.Found.ToList(),
      CardAPI = CardAPI,
      NextPage = result.NextPageUri,
    };
    SearchCards = new(source, CardAPI.PageSize);
    IsBusy = false;
  }
}
