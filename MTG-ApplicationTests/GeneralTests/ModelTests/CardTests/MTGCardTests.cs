using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.General.Models.MTGCardInfo;

namespace MTGApplicationTests.GeneralTests.ModelTests.CardTests;

[TestClass]
public class MTGCardTests
{
  [TestMethod]
  public void SpellTypes_AllTypes()
  {
    var allTypeLine = string.Join(' ', Enum.GetNames(typeof(SpellType)));

    var card = DeckEditorMTGCardMocker.CreateMTGCardModel(typeLine: allTypeLine);

    CollectionAssert.AreEquivalent(Enum.GetValues<SpellType>().ToArray(), card.Info.SpellTypes);
  }

  [TestMethod]
  public void ColorTypes_TwoFaces_AllColors()
  {
    var frontFace = DeckEditorMTGCardMocker.CreateCardFace(colors: [ColorTypes.W, ColorTypes.U, ColorTypes.B]);
    var backFace = DeckEditorMTGCardMocker.CreateCardFace(colors: [ColorTypes.R, ColorTypes.G]);
    var card = DeckEditorMTGCardMocker.CreateMTGCardModel(frontFace: frontFace, backFace: backFace);

    CollectionAssert.AreEquivalent(
      new ColorTypes[] { ColorTypes.W, ColorTypes.U, ColorTypes.B, ColorTypes.R, ColorTypes.G }, card.Info.Colors);
  }

  [TestMethod]
  public void ColorTypes_NoColors_Colorless()
  {
    var frontFace = DeckEditorMTGCardMocker.CreateCardFace(colors: []);
    var card = DeckEditorMTGCardMocker.CreateMTGCardModel(frontFace: frontFace);

    CollectionAssert.AreEquivalent(new ColorTypes[] { ColorTypes.C }, card.Info.Colors);
  }

  [TestMethod]
  public void Serialize()
  {
    var card = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

    JsonExtensions.TrySerializeObject(card, out var serialized);

    Assert.IsNotNull(serialized);
  }

  [TestMethod]
  public void Deserialize()
  {
    var card = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

    JsonExtensions.TrySerializeObject(card, out var serialized);
    JsonExtensions.TryDeserializeJson(serialized, out DeckEditorMTGCard deserialized);

    Assert.IsNotNull(deserialized);
    Assert.AreEqual(card.Info.Name, deserialized.Info.Name);
    Assert.AreEqual(card.Count, deserialized.Count);
  }

  [TestMethod("Card cound should be the given value if the value is more than one")]
  public void Count_MoreThanOne_IsValue()
  {
    var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
    var countValue = 10;

    card.Count = countValue;

    Assert.AreEqual(countValue, card.Count);
  }

  [TestMethod("Card cound should be the one if the value is less than one")]
  public void Count_LessThanOne_IsOne()
  {
    var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
    var countValue = -10;

    card.Count = countValue;

    Assert.AreEqual(1, card.Count);
  }
}