using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.GeneralTests.ModelTests.CardDeckTests;
[TestClass]
public class CardDeckTests
{
  [TestMethod]
  public void DeckPrice_HasCards_PriceIsCardPriceMultipliedByCount()
  {
    var deck = new DeckEditorMTGDeck()
    {
      DeckCards = [
        DeckEditorMTGCardMocker.CreateMTGCardModel(count: 1, price: 1),
        DeckEditorMTGCardMocker.CreateMTGCardModel(count: 2, price: 2),
        ],
    };

    Assert.AreEqual(deck.DeckCards.Sum(x => x.Info.Price * x.Count), deck.DeckPrice);
  }

  [TestMethod]
  public void DeckPrice_HasCommander_PriceIsCommanderPrice()
  {
    var deck = new DeckEditorMTGDeck()
    {
      Commander = DeckEditorMTGCardMocker.CreateMTGCardModel(count: 2, price: 8),
    };

    Assert.AreEqual(deck.Commander.Info.Price, deck.DeckPrice);
  }

  [TestMethod]
  public void DeckPrice_HasPartner_PriceIsPartnerPrice()
  {
    var deck = new DeckEditorMTGDeck()
    {
      CommanderPartner = DeckEditorMTGCardMocker.CreateMTGCardModel(count: 2, price: 8),
    };

    Assert.AreEqual(deck.CommanderPartner.Info.Price, deck.DeckPrice);
  }
}
