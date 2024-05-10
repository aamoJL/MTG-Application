using MTGApplication.General.Models;
using MTGApplication.General.Models.Card;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.API.CardAPI;
public class GetMTGCardsBySearchQuery : UseCase<string, Task<ICardAPI<MTGCard>.Result>>
{
  public GetMTGCardsBySearchQuery(ICardAPI<MTGCard> cardAPI) => CardAPI = cardAPI;

  private ICardAPI<MTGCard> CardAPI { get; }

  public IWorker Worker { get; init; } = new DefaultWorker();

  public async override Task<ICardAPI<MTGCard>.Result> Execute(string query)
    => await Worker.DoWork(CardAPI.FetchCardsWithSearchQuery(query));
}