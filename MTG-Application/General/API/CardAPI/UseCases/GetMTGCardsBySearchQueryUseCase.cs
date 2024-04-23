using MTGApplication.General.UseCases;
using MTGApplication.Models;
using System.Threading.Tasks;

namespace MTGApplication.API.CardAPI;
class GetMTGCardsBySearchQueryUseCase : UseCase<Task<ICardAPI<MTGCard>.Result>>
{
  public GetMTGCardsBySearchQueryUseCase(ICardAPI<MTGCard> cardAPI, string query)
  {
    CardAPI = cardAPI;
    Query = query;
  }

  public ICardAPI<MTGCard> CardAPI { get; }
  public string Query { get; }

  public async override Task<ICardAPI<MTGCard>.Result> Execute()
    => await CardAPI.FetchCardsWithSearchQuery(Query);
}