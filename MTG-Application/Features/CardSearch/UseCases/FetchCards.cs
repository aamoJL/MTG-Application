using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.API.CardAPI.UseCases;

namespace MTGApplication.Features.CardSearch.UseCases;

public class FetchCards(ICardImporter<DeckEditorMTGCard> cardAPI) : FetchCardsWithSearchQuery(cardAPI)
{
}
