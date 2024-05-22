using MTGApplication.General.Models.CardDeck;

namespace MTGApplicationTests.TestUtility.Mocker;

public static class MTGCardDeckMocker
{
  public static MTGCardDeck Mock(string name, bool includeCommander = true, bool includePartner = false)
  {
#pragma warning disable CS8601 // Possible null reference assignment.
    return new()
    {
      Name = name,
      Commander = includeCommander ? MTGCardModelMocker.CreateMTGCardModel(name: "Commander") : null,
      CommanderPartner = includePartner ? MTGCardModelMocker.CreateMTGCardModel(name: "Partner") : null,
      DeckCards = [MTGCardModelMocker.CreateMTGCardModel(name: "First"), MTGCardModelMocker.CreateMTGCardModel(name: "Second")],
      Wishlist = [MTGCardModelMocker.CreateMTGCardModel(name: "First")],
      Maybelist = [MTGCardModelMocker.CreateMTGCardModel(name: "First")],
      Removelist = [MTGCardModelMocker.CreateMTGCardModel(name: "First")]
    };
#pragma warning restore CS8601 // Possible null reference assignment.
  }
}