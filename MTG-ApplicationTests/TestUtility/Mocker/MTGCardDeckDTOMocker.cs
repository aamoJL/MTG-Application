using MTGApplication.General.Models.CardDeck;

namespace MTGApplicationTests.Services;

public static class MTGCardDeckDTOMocker
{
  public static MTGCardDeckDTO Mock(string name, bool mockCommander = true)
  {
    return new(
      name: name,
      commander: mockCommander ? new(
        scryfallId: new("4f8dc511-e307-4412-bb79-375a6077312d"),
        oracleId: new("8095ca78-db19-4724-a6ff-eacc85fa2274"),
        setCode: "otj",
        collectorNumber: "1") : null
      );
  }

  public static IEnumerable<MTGCardDeckDTO> MockList(int count)
  {
    var list = new List<MTGCardDeckDTO>();
    list.AddRange(Enumerable.Range(1, count).Select(x => Mock($"Deck {x}")));
    return list;
  }
}
