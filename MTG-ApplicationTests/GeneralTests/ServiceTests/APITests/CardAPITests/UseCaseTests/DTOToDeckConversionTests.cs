using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.GeneralTests.Services.APITests.CardAPITests.UseCaseTests;
[TestClass]
public class DTOToDeckConversionTests
{
  [TestMethod]
  public async Task Execute_DTOConvertedToDeck()
  {
    var cards = new DeckEditorMTGCard[]
    {
        MTGCardModelMocker.CreateMTGCardModel(name: "first"),
        MTGCardModelMocker.CreateMTGCardModel(name: "second"),
        MTGCardModelMocker.CreateMTGCardModel(name: "third"),
    };
    var deck = new MTGCardDeck()
    {
      Name = "First",
      DeckCards = new(cards),
    };

    var result = await new DTOToDeckConverter(new TestCardAPI()
    {
      ExpectedCards = cards
    }).Execute(new(deck));

    Assert.IsNotNull(result);
    Assert.AreEqual(3, result.DeckCards.Count);
    CollectionAssert.AreEquivalent(
      deck.DeckCards.Select(x => x.Info).ToArray(),
      result.DeckCards.Select(x => x.Info).ToArray());
  }
}
