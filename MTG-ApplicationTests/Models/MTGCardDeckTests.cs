using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.Models;

[TestClass]
public class MTGCardDeckTests
{
  [TestMethod]
  public void AddToCardlistTest()
  {
    var deck = new MTGCardDeck();

    MTGCardDeck.AddOrCombineToCardlist(new[] { Mocker.MTGCardModelMocker.CreateMTGCardModel() }, deck.DeckCards);
    Assert.AreEqual(1, deck.DeckCards.Count);
  }

  [TestMethod]
  public void RemoveFromCardlistTest()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var deck = new MTGCardDeck() { DeckCards = new() { card } };

    MTGCardDeck.RemoveOrReduceFromCardlist(new[] { card }, deck.DeckCards);
    Assert.AreEqual(0, deck.DeckCards.Count);
  }

  [TestMethod]
  public async Task MTGCardDeckDTO_AsMTGCardDeckTest()
  {
    var name = "First";
    ObservableCollection<MTGCard> deckCards = new()
    {
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
    };
    var deck = new MTGCardDeck() { Name = name, DeckCards = deckCards };
    var dto = new MTGCardDeckDTO(deck);

    var dtoToDeck = await dto.AsMTGCardDeck(new TestCardAPI(deckCards.ToArray()));
    Assert.AreEqual(name, dtoToDeck.Name);
    CollectionAssert.AreEquivalent(deckCards.Select(x => x.Info.ScryfallId).ToArray(), dtoToDeck.DeckCards.Select(x => x.Info.ScryfallId).ToArray());
  }
}
