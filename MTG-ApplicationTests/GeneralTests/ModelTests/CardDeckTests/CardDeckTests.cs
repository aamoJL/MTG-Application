using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.GeneralTests.ModelTests.CardDeckTests;
[TestClass]
public class CardDeckTests
{
  [TestMethod]
  public void DeckPrice_HasCards_PriceIsCardPriceMultipliedByCount()
  {
    var deck = new MTGCardDeck()
    {
      DeckCards = [
        MTGCardModelMocker.CreateMTGCardModel(count: 1, price: 1),
        MTGCardModelMocker.CreateMTGCardModel(count: 2, price: 2),
        ],
    };

    Assert.AreEqual(deck.DeckCards.Sum(x => x.Info.Price * x.Count), deck.DeckPrice);
  }

  [TestMethod]
  public void DeckPrice_HasCommander_PriceIsCommanderPrice()
  {
    var deck = new MTGCardDeck()
    {
      Commander = MTGCardModelMocker.CreateMTGCardModel(count: 2, price: 8),
    };

    Assert.AreEqual(deck.Commander.Info.Price, deck.DeckPrice);
  }

  [TestMethod]
  public void DeckPrice_HasPartner_PriceIsPartnerPrice()
  {
    var deck = new MTGCardDeck()
    {
      CommanderPartner = MTGCardModelMocker.CreateMTGCardModel(count: 2, price: 8),
    };

    Assert.AreEqual(deck.CommanderPartner.Info.Price, deck.DeckPrice);
  }
}
