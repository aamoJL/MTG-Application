using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplicationTests.Services;
using static MTGApplication.General.Models.Card.CardSortProperties;

namespace MTGApplicationTests.GeneralTests.ModelTests.Card;
public class CardSortPropertiesTests
{
  [TestClass]
  public class MTGCardComparerTests
  {
    [TestMethod]
    public void Compare()
    {
      var card1 = Mocker.MTGCardModelMocker.CreateMTGCardModel(
        name: "A", cmc: 1, rarity: MTGCard.RarityTypes.Common,
        setCode: "aaa", count: 1, price: 1, typeLine: MTGCard.SpellType.Land.ToString(),
        frontFace: Mocker.MTGCardModelMocker.CreateCardFace(colors: [MTGCard.ColorTypes.W]));
      var card2 = Mocker.MTGCardModelMocker.CreateMTGCardModel(
        name: "B", cmc: 2, rarity: MTGCard.RarityTypes.Rare,
        setCode: "bbb", count: 2, price: 2, typeLine: MTGCard.SpellType.Creature.ToString(),
        frontFace: Mocker.MTGCardModelMocker.CreateCardFace(colors: [MTGCard.ColorTypes.U]));

      foreach (var property in Enum.GetNames(typeof(MTGSortProperty)))
      {
        var comparer = new MTGCardPropertyComparer(Enum.Parse<MTGSortProperty>(property));
        Assert.AreEqual(1, comparer.Compare(card2, card1), $"{property} was not compared correctly");
      };
    }
  }
}
