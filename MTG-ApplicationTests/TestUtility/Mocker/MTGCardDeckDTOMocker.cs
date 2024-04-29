using MTGApplication.General.Models.CardDeck;

namespace MTGApplicationTests.Services;

public static class MTGCardDeckDTOMocker
{
  public static MTGCardDeckDTO Mock(string name, bool mockCommander = true)
  {
    return new(
      name: name,
      commander: mockCommander ? MTGCardDTOMocker.Mock("Commander") : null);
  }

  public static IEnumerable<MTGCardDeckDTO> MockList(int count)
  {
    var list = new List<MTGCardDeckDTO>();
    list.AddRange(Enumerable.Range(1, count).Select(x => Mock($"Deck {x}")));
    return list;
  }
}