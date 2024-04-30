using MTGApplication.General.Models.CardDeck;

namespace MTGApplicationTests.Services;

public static class MTGCardDeckMocker
{
  public static MTGCardDeck Mock(string name, bool includeCommander = true, bool includePartner = false)
  {
#pragma warning disable CS8601 // Possible null reference assignment.
    return new()
    {
      Name = name,
      Commander = includeCommander ? Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Commander") : null,
      CommanderPartner = includePartner ? Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Partner") : null,
      DeckCards = [Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"), Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second")],
      Wishlist = [Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First")],
      Maybelist = [Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First")],
      Removelist = [Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First")]
    };
#pragma warning restore CS8601 // Possible null reference assignment.
  }
}