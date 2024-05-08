using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardDeck;
using MTGApplication.Services;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class CardFiltersTests
{
  [TestMethod]
  public void ResetFilters()
  {
    var filters = new CardFilters()
    {
      NameText = "Name",
      TypeText = "Type",
      OracleText = "Oracle",
      White = false,
      Blue = false,
      Black = false,
      Red = false,
      Green = false,
      Colorless = false,
      ColorGroup = CardFilters.ColorGroups.Multi,
      Cmc = 3,
    };

    Assert.IsTrue(filters.ResetCommand.CanExecute(null));
    filters.ResetCommand.Execute(null);

    var expectedFilters = new CardFilters();

    Assert.AreEqual(expectedFilters.NameText, filters.NameText);
    Assert.AreEqual(expectedFilters.TypeText, filters.TypeText);
    Assert.AreEqual(expectedFilters.OracleText, filters.OracleText);
    Assert.AreEqual(expectedFilters.White, filters.White);
    Assert.AreEqual(expectedFilters.Blue, filters.Blue);
    Assert.AreEqual(expectedFilters.Black, filters.Black);
    Assert.AreEqual(expectedFilters.Red, filters.Red);
    Assert.AreEqual(expectedFilters.Green, filters.Green);
    Assert.AreEqual(expectedFilters.Colorless, filters.Colorless);
    Assert.AreEqual(expectedFilters.ColorGroup, filters.ColorGroup);
    Assert.AreEqual(expectedFilters.Cmc, filters.Cmc);
  }

  [TestMethod]
  public void FiltersApplied()
  {
    Assert.IsFalse(new CardFilters().FiltersApplied);
    Assert.IsTrue(new CardFilters() { NameText = "Text" }.FiltersApplied);
    Assert.IsTrue(new CardFilters() { TypeText = "Text" }.FiltersApplied);
    Assert.IsTrue(new CardFilters() { OracleText = "Text" }.FiltersApplied);
    Assert.IsTrue(new CardFilters() { White = false }.FiltersApplied);
    Assert.IsTrue(new CardFilters() { Blue = false }.FiltersApplied);
    Assert.IsTrue(new CardFilters() { Black = false }.FiltersApplied);
    Assert.IsTrue(new CardFilters() { Red = false }.FiltersApplied);
    Assert.IsTrue(new CardFilters() { Green = false }.FiltersApplied);
    Assert.IsTrue(new CardFilters() { Colorless = false }.FiltersApplied);
    Assert.IsTrue(new CardFilters() { ColorGroup = CardFilters.ColorGroups.Multi }.FiltersApplied);
    Assert.IsTrue(new CardFilters() { ColorGroup = CardFilters.ColorGroups.Mono }.FiltersApplied);
    Assert.IsTrue(new CardFilters() { Cmc = 3 }.FiltersApplied);
  }

  [TestMethod]
  public void ChangeColorGroup()
  {
    var filters = new CardFilters();

    var multi = CardFilters.ColorGroups.Multi;
    filters.ChangeColorGroupCommand.Execute(multi.ToString());
    Assert.AreEqual(multi, filters.ColorGroup);

    var all = CardFilters.ColorGroups.All;
    filters.ChangeColorGroupCommand.Execute(all.ToString());
    Assert.AreEqual(all, filters.ColorGroup);

    var mono = CardFilters.ColorGroups.Mono;
    filters.ChangeColorGroupCommand.Execute(mono.ToString());
    Assert.AreEqual(mono, filters.ColorGroup);
  }

  [TestMethod]
  public void CardValidation()
  {
    Assert.IsTrue(new CardFilters().CardValidation(Mocker.MTGCardModelMocker.CreateMTGCardModel()));
    Assert.IsFalse(new CardFilters() { NameText = "A" }.CardValidation(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "B")));
    Assert.IsFalse(new CardFilters() { TypeText = "A" }.CardValidation(Mocker.MTGCardModelMocker.CreateMTGCardModel(typeLine: "B")));
    Assert.IsFalse(new CardFilters() { OracleText = "A" }.CardValidation(Mocker.MTGCardModelMocker.CreateMTGCardModel(frontFace:
      Mocker.MTGCardModelMocker.CreateCardFace(oracleText: "B"))));
    Assert.IsFalse(new CardFilters() { White = false }.CardValidation(Mocker.MTGCardModelMocker.CreateMTGCardModel(frontFace:
      Mocker.MTGCardModelMocker.CreateCardFace(colors: [MTGService.ColorTypes.W]))));
    Assert.IsFalse(new CardFilters() { Blue = false }.CardValidation(Mocker.MTGCardModelMocker.CreateMTGCardModel(frontFace:
      Mocker.MTGCardModelMocker.CreateCardFace(colors: [MTGService.ColorTypes.U]))));
    Assert.IsFalse(new CardFilters() { Black = false }.CardValidation(Mocker.MTGCardModelMocker.CreateMTGCardModel(frontFace:
      Mocker.MTGCardModelMocker.CreateCardFace(colors: [MTGService.ColorTypes.B]))));
    Assert.IsFalse(new CardFilters() { Red = false }.CardValidation(Mocker.MTGCardModelMocker.CreateMTGCardModel(frontFace:
      Mocker.MTGCardModelMocker.CreateCardFace(colors: [MTGService.ColorTypes.R]))));
    Assert.IsFalse(new CardFilters() { Green = false }.CardValidation(Mocker.MTGCardModelMocker.CreateMTGCardModel(frontFace:
      Mocker.MTGCardModelMocker.CreateCardFace(colors: [MTGService.ColorTypes.G]))));
    Assert.IsFalse(new CardFilters() { Colorless = false }.CardValidation(Mocker.MTGCardModelMocker.CreateMTGCardModel(frontFace:
      Mocker.MTGCardModelMocker.CreateCardFace(colors: [MTGService.ColorTypes.C]))));
    Assert.IsTrue(new CardFilters() { ColorGroup = CardFilters.ColorGroups.All }.CardValidation(Mocker.MTGCardModelMocker.CreateMTGCardModel(frontFace:
      Mocker.MTGCardModelMocker.CreateCardFace(colors: [MTGService.ColorTypes.U]))));
    Assert.IsFalse(new CardFilters() { ColorGroup = CardFilters.ColorGroups.Multi }.CardValidation(Mocker.MTGCardModelMocker.CreateMTGCardModel(frontFace:
      Mocker.MTGCardModelMocker.CreateCardFace(colors: [MTGService.ColorTypes.U]))));
    Assert.IsFalse(new CardFilters() { ColorGroup = CardFilters.ColorGroups.Mono }.CardValidation(Mocker.MTGCardModelMocker.CreateMTGCardModel(frontFace:
      Mocker.MTGCardModelMocker.CreateCardFace(colors: [MTGService.ColorTypes.W, MTGService.ColorTypes.U]))));
  }
}