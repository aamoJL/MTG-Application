using MTGApplication.General.Models.Card;
using MTGApplication.General.UseCases;
using MTGApplication.General.ViewModels;
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
  public IWorker Worker { get; init; } = new DefaultWorker();

  public async override Task<ICardAPI<MTGCard>.Result> Execute()
    => await Worker.DoWork(CardAPI.FetchCardsWithSearchQuery(Query));
}