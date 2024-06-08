﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.General.Models.Card.CardSortProperties;

namespace MTGApplicationTests.GeneralTests.ModelTests.CardTests;
public class CardSortPropertiesTests
{
  [TestClass]
  public class MTGCardComparerTests
  {
    [TestMethod]
    public void Compare()
    {
      var card1 = MTGCardModelMocker.CreateMTGCardModel(
        name: "A", cmc: 1, rarity: DeckEditorMTGCard.RarityTypes.Common,
        setCode: "aaa", count: 1, price: 1, typeLine: DeckEditorMTGCard.SpellType.Land.ToString(),
        frontFace: MTGCardModelMocker.CreateCardFace(colors: [DeckEditorMTGCard.ColorTypes.W]));
      var card2 = MTGCardModelMocker.CreateMTGCardModel(
        name: "B", cmc: 2, rarity: DeckEditorMTGCard.RarityTypes.Rare,
        setCode: "bbb", count: 2, price: 2, typeLine: DeckEditorMTGCard.SpellType.Creature.ToString(),
        frontFace: MTGCardModelMocker.CreateCardFace(colors: [DeckEditorMTGCard.ColorTypes.U]));

      foreach (var property in Enum.GetNames(typeof(MTGSortProperty)))
      {
        var comparer = new MTGCardPropertyComparer(Enum.Parse<MTGSortProperty>(property));
        Assert.AreEqual(1, comparer.Compare(card2, card1), $"{property} was not compared correctly");
      };
    }
  }
}
