using MTGApplication.Features.CardSearch.Models;
using MTGApplication.General.Services.API.CardAPI.UseCases;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch.UseCases;

public partial class CardSearchViewModelCommands
{
  public class SubmitSearch(CardSearchViewModel viewmodel) : ViewModelAsyncCommand<CardSearchViewModel, string>(viewmodel)
  {
    protected override async Task Execute(string query)
    {
      var searchResult = await Viewmodel.Worker.DoWork(new FetchCardsWithSearchQuery(Viewmodel.Importer).Execute(query));

      Viewmodel.Cards.SetCollection(
        cards: [.. searchResult.Found.Select(x => new CardSearchMTGCard(x.Info))],
        nextPageUri: searchResult.NextPageUri,
        totalCount: searchResult.TotalCount);
    }
  }
}
