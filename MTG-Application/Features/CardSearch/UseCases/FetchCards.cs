using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;

namespace MTGApplication.Features.CardSearch;

public class FetchCards : GetMTGCardsBySearchQuery
{
  public FetchCards(ICardAPI<MTGCard> cardAPI) : base(cardAPI) { }
}
