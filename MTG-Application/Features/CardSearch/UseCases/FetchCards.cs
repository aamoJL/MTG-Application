using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;

namespace MTGApplication.Features.CardSearch;

public class FetchCards(ICardAPI<MTGCard> cardAPI) : GetMTGCardsBySearchQuery(cardAPI)
{
}
