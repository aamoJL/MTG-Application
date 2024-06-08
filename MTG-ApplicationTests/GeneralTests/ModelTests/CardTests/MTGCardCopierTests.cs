using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.GeneralTests.ModelTests.CardTests;

[TestClass]
public class MTGCardCopierTests
{
  [TestMethod]
  public void Copy_Single()
  {
    var card = MTGCardModelMocker.CreateMTGCardModel(name: "card", cmc: 2, count: 5);
    var copier = new DeckEditorMTGCardCopier();

    var result = copier.Copy(card);

    Assert.AreEqual(
      (card.Info.Name, card.Info.CMC, card.Count),
      (result.Info.Name, result.Info.CMC, result.Count));
  }

  [TestMethod]
  public void Copy_Multiple()
  {
    var cards = new List<DeckEditorMTGCard>()
    {
      MTGCardModelMocker.CreateMTGCardModel(name: "card1", cmc: 1, count: 1),
      MTGCardModelMocker.CreateMTGCardModel(name: "card2", cmc: 2, count: 2),
      MTGCardModelMocker.CreateMTGCardModel(name: "card3", cmc: 3, count: 3)
    };
    var copier = new DeckEditorMTGCardCopier();

    var result = copier.Copy(cards);

    CollectionAssert.AreEquivalent(cards.Select(x => x.Info.Name).ToList(), result.Select(x => x.Info.Name).ToList());
    CollectionAssert.AreEquivalent(cards.Select(x => x.Info.CMC).ToList(), result.Select(x => x.Info.CMC).ToList());
    CollectionAssert.AreEquivalent(cards.Select(x => x.Count).ToList(), result.Select(x => x.Count).ToList());
  }
}