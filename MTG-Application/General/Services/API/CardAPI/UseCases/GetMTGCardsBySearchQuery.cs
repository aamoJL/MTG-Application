using MTGApplication.General.Models.Card;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.API.CardAPI;
public class GetMTGCardsBySearchQuery : UseCase<string, Task<CardImportResult>>
{
  public GetMTGCardsBySearchQuery(ICardAPI<MTGCard> cardAPI) => CardAPI = cardAPI;

  private ICardAPI<MTGCard> CardAPI { get; }

  public async override Task<CardImportResult> Execute(string query)
    => await CardAPI.FetchCardsWithSearchQuery(query);
}