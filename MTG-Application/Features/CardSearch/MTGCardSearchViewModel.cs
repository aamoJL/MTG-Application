using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.API.CardAPI;
using MTGApplication.General.Models.Card;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch;
/// <summary>
/// ViewModel for <see cref="MTGCardSearchView"/>
/// </summary>
public partial class MTGCardSearchViewModel : ViewModelBase, IWorker
{
  public MTGCardSearchViewModel(ICardAPI<MTGCard> cardAPI)
  {
    CardAPI = cardAPI;
    Cards = new(CardAPI);
  }

  public ICardAPI<MTGCard> CardAPI { get; }
  public IncrementalLoadingCardCollection Cards { get; }

  [ObservableProperty] private bool isBusy;

  [RelayCommand]
  private async Task SubmitSearch(string query)
  {
    var searchResult = await new GetMTGCardsBySearchQueryUseCase(CardAPI, query) { Worker = this }.Execute();

    Cards.SetCollection(searchResult.Found.ToList(), searchResult.NextPageUri, searchResult.TotalCount);
  }
}
