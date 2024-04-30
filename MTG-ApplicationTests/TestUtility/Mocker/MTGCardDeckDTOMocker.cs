using MTGApplication.General.Models.CardDeck;

namespace MTGApplicationTests.Services;

public static class MTGCardDeckDTOMocker
{
  public static MTGCardDeckDTO Mock(string name, bool includeCommander = true, bool includePartner = true)
  {
    return new(name)
    {
      DeckCards = [MTGCardDTOMocker.Mock("first"), MTGCardDTOMocker.Mock("second"), MTGCardDTOMocker.Mock("third")],
      WishlistCards = [MTGCardDTOMocker.Mock("first")],
      MaybelistCards = [MTGCardDTOMocker.Mock("first"), MTGCardDTOMocker.Mock("second")],
      RemovelistCards = [MTGCardDTOMocker.Mock("first")],
      Commander = includeCommander ? MTGCardDTOMocker.Mock("Commander") : null,
      CommanderPartner = includePartner ? MTGCardDTOMocker.Mock("Partner") : null
    };
  }

  public static IEnumerable<MTGCardDeckDTO> MockList(int count)
  {
    var list = new List<MTGCardDeckDTO>();
    list.AddRange(Enumerable.Range(1, count).Select(x => Mock($"Deck {x}")));
    return list;
  }
}