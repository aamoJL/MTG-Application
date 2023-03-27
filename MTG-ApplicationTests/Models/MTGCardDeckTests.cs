using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;
using System.Collections.ObjectModel;
using static MTGApplication.Enums;

namespace MTGApplicationTests.Models
{
  [TestClass]
  public class MTGCardDeckTests
  {
    [TestMethod]
    public void GetCardlistTest()
    {
      var deck = new MTGCardDeck();

      deck.AddToCardlist(CardlistType.Deck, Mocker.MTGCardModelMocker.CreateMTGCardModel());
      Assert.AreEqual(1, deck.GetCardlist(CardlistType.Deck).Count);
    }

    [TestMethod]
    public void AddToCardlistTest()
    {
      var deck = new MTGCardDeck();
      
      deck.AddToCardlist(CardlistType.Deck, Mocker.MTGCardModelMocker.CreateMTGCardModel());
      Assert.AreEqual(1, deck.DeckCards.Count);
    }

    [TestMethod]
    public void RemoveFromCardlistTest()
    {
      var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
      var deck = new MTGCardDeck() { DeckCards = new() { card } };

      deck.RemoveFromCardlist(CardlistType.Deck, card);
      Assert.AreEqual(0, deck.DeckCards.Count);
    }

    [TestMethod]
    public async Task MTGCardDeckDTO_AsMTGCardDeckTest()
    {
      string name = "First";
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
}
