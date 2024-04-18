using MTGApplication.Interfaces;
using MTGApplication.Models;
using System.Threading.Tasks;

namespace MTGApplication.Features.MTGCardSearch.UseCases;
class GetMTGCardsBySearchQueryUseCase : UseCase<string, Task<ICardAPI<MTGCard>.Result>>
{
  public ICardAPI<MTGCard> CardAPI { get; }

  public GetMTGCardsBySearchQueryUseCase(ICardAPI<MTGCard> cardAPI) => CardAPI = cardAPI;

  public async override Task<ICardAPI<MTGCard>.Result> Execute(string args) 
    => (await CardAPI.FetchCardsWithSearchQuery(args));
}