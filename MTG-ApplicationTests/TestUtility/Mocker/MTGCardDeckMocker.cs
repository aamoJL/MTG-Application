using MTGApplication.Features.DeckEditor.Models;

namespace MTGApplicationTests.TestUtility.Mocker;

public static class MTGCardDeckMocker
{
  public static DeckEditorMTGDeck Mock(string name, bool includeCommander = true, bool includePartner = false)
  {
#pragma warning disable CS8601 // Possible null reference assignment.
    return new()
    {
      Name = name,
      Commander = includeCommander ? DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Commander") : null,
      CommanderPartner = includePartner ? DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Partner") : null,
      DeckCards = [DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First"), DeckEditorMTGCardMocker.CreateMTGCardModel(name: "Second")],
      Wishlist = [DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First")],
      Maybelist = [DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First")],
      Removelist = [DeckEditorMTGCardMocker.CreateMTGCardModel(name: "First")]
    };
#pragma warning restore CS8601 // Possible null reference assignment.
  }
}