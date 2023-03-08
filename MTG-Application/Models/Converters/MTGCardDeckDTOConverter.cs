using MTGApplication.Interfaces;
using System.Threading.Tasks;

namespace MTGApplication.Models.Converters
{
  public static class MTGCardDeckDTOConverter
  {
    public static async Task<MTGCardDeck> Convert(MTGCardDeckDTO dto, ICardAPI<MTGCard> api)
    {
      if (dto == null) { return null; }
      return new MTGCardDeck()
      {
        Name = dto.Name,
        DeckCards = new((await api.FetchFromDTOs(dto.DeckCards.ToArray())).Found),
        Wishlist = new((await api.FetchFromDTOs(dto.WishlistCards.ToArray())).Found),
        Maybelist = new((await api.FetchFromDTOs(dto.MaybelistCards.ToArray())).Found),
      };
    }
  }
}
