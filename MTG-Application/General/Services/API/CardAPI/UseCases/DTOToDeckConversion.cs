using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.API.CardAPI;

public class DTOToDeckConversion : UseCase<MTGCardDeckDTO, Task<MTGCardDeck>>
{
  public DTOToDeckConversion(ICardAPI<MTGCard> cardAPI) => CardAPI = cardAPI;

  public ICardAPI<MTGCard> CardAPI { get; }

  public override async Task<MTGCardDeck> Execute(MTGCardDeckDTO dto)
  {
    return new MTGCardDeck()
    {
      Name = dto.Name,
      Commander = dto.Commander != null ? (await CardAPI.FetchFromDTOs(new CardDTO[] { dto.Commander })).Found.FirstOrDefault() : null,
      CommanderPartner = dto.CommanderPartner != null ? (await CardAPI.FetchFromDTOs(new CardDTO[] { dto.CommanderPartner })).Found.FirstOrDefault() : null,
      DeckCards = new((await CardAPI.FetchFromDTOs(dto.DeckCards.ToArray())).Found),
      Wishlist = new((await CardAPI.FetchFromDTOs(dto.WishlistCards.ToArray())).Found),
      Maybelist = new((await CardAPI.FetchFromDTOs(dto.MaybelistCards.ToArray())).Found),
      Removelist = new((await CardAPI.FetchFromDTOs(dto.RemovelistCards.ToArray())).Found),
    };
  }
}
