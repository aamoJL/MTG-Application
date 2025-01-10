using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.Features.DeckEditor.CardList.Services.CardSortProperties;
using static MTGApplication.General.Models.MTGCardInfo;

namespace MTGApplicationTests.GeneralTests.ModelTests.CardTests;
public class CardSortPropertiesTests
{
  [TestClass]
  public class MTGCardComparerTests
  {
    [TestMethod]
    public void Compare()
    {
      var card1 = DeckEditorMTGCardMocker.CreateMTGCardModel(
        name: "A", cmc: 1, rarity: RarityTypes.Common,
        setCode: "aaa", count: 1, price: 1, typeLine: SpellType.Land.ToString(),
        frontFace: DeckEditorMTGCardMocker.CreateCardFace(colors: [ColorTypes.W]));
      var card2 = DeckEditorMTGCardMocker.CreateMTGCardModel(
        name: "B", cmc: 2, rarity: RarityTypes.Rare,
        setCode: "bbb", count: 2, price: 2, typeLine: SpellType.Creature.ToString(),
        frontFace: DeckEditorMTGCardMocker.CreateCardFace(colors: [ColorTypes.U]));

      foreach (var property in Enum.GetNames<MTGSortProperty>())
      {
        var comparer = new MTGCardPropertyComparer([Enum.Parse<MTGSortProperty>(property)], CommunityToolkit.WinUI.Collections.SortDirection.Ascending);
        Assert.AreEqual(1, comparer.Compare(card2, card1), $"{property} was not compared correctly");
      };
    }
  }
}
