using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using MTGApplication.API.CardAPI;
using MTGApplication.General.ViewModels;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch;
/// <summary>
/// ViewModel for <see cref="MTGCardSearchView"/>
/// </summary>
public partial class MTGCardSearchViewModel : ViewModelBase
{
  public MTGCardSearchViewModel(ICardAPI<MTGCard> cardAPI) => CardAPI = cardAPI;

  public ICardAPI<MTGCard> CardAPI { get; }

  [ObservableProperty] private IncrementalLoadingCollection<MTGCardViewModelSource, MTGCardViewModel> cards = new();
  [ObservableProperty] private int totalCardCount;
  [ObservableProperty] private bool isBusy;

  [RelayCommand]
  public async Task SubmitSearch(string query)
  {
    IsBusy = true;
    var searchResult = await new GetMTGCardsBySearchQueryUseCase(CardAPI, query).Execute();
    TotalCardCount = searchResult.TotalCount;

    Cards = new(
      itemsPerPage: CardAPI.PageSize,
      source: new MTGCardViewModelSource()
      {
        Cards = searchResult.Found.ToList(),
        CardAPI = CardAPI,
        NextPage = searchResult.NextPageUri,
      });
    IsBusy = false;
  }
}
