using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCard;
using static MTGApplication.Features.DeckEditor.Services.CardSortProperties;
using static MTGApplication.General.Models.MTGCardInfo;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.Services;

[TestClass]
public class CardSortProperties
{
  [TestMethod]
  public void Compare()
  {
    var factory = new TestDeckCardViewModelFactory();

    var card1 = factory.Build(DeckEditorMTGCardMocker.CreateMTGCardModel(
      name: "A",
      cmc: 1,
      rarity: RarityTypes.Common,
      setCode: "aaa",
      count: 1,
      price: 1,
      typeLine: SpellType.Land.ToString(),
      frontFace: DeckEditorMTGCardMocker.CreateCardFace(colors: [ColorTypes.W])));
    var card2 = factory.Build(DeckEditorMTGCardMocker.CreateMTGCardModel(
      name: "B",
      cmc: 2,
      rarity: RarityTypes.Rare,
      setCode: "bbb",
      count: 2,
      price: 2,
      typeLine: SpellType.Creature.ToString(),
      frontFace: DeckEditorMTGCardMocker.CreateCardFace(colors: [ColorTypes.U])));

    foreach (var property in Enum.GetNames<MTGSortProperty>())
    {
      var comparer = new MTGCardPropertyComparer([Enum.Parse<MTGSortProperty>(property)], CommunityToolkit.WinUI.Collections.SortDirection.Ascending);
      Assert.AreEqual(1, comparer.Compare(card2, card1), $"{property} was not compared correctly");
    }
  }
}
