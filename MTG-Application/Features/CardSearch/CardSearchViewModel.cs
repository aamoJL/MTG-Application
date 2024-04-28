using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch;
/// <summary>
/// ViewModel for <see cref="CardSearchView"/>
/// </summary>
public partial class CardSearchViewModel : ViewModelBase, IWorker
{
  public CardSearchViewModel(ICardAPI<MTGCard> cardAPI)
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
    var searchResult = await new GetMTGCardsBySearchQuery(CardAPI) { Worker = this }.Execute(query);

    Cards.SetCollection(searchResult.Found.ToList(), searchResult.NextPageUri, searchResult.TotalCount);
  }
}
