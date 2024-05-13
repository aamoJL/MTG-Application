using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.IOService;
using MTGApplicationTests.Services;
using static MTGApplication.General.Models.Card.MTGCard;

namespace MTGApplicationTests.Models;

[TestClass]
public class MTGCardTests
{
  [TestMethod]
  public void SpellTypes_AllTypes()
  {
    var allTypeLine = string.Join(' ', Enum.GetNames(typeof(SpellType)));

    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(typeLine: allTypeLine);

    CollectionAssert.AreEquivalent(Enum.GetValues<SpellType>().ToArray(), card.Info.SpellTypes);
  }

  [TestMethod]
  public void ColorTypes_TwoFaces_AllColors()
  {
    var frontFace = Mocker.MTGCardModelMocker.CreateCardFace(colors: [ColorTypes.W, ColorTypes.U, ColorTypes.B]);
    var backFace = Mocker.MTGCardModelMocker.CreateCardFace(colors: [ColorTypes.R, ColorTypes.G]);
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(frontFace: frontFace, backFace: backFace);

    CollectionAssert.AreEquivalent(
      new ColorTypes[] { ColorTypes.W, ColorTypes.U, ColorTypes.B, ColorTypes.R, ColorTypes.G }, card.Info.Colors);
  }

  [TestMethod]
  public void ColorTypes_NoColors_Colorless()
  {
    var frontFace = Mocker.MTGCardModelMocker.CreateCardFace(colors: []);
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel(frontFace: frontFace);

    CollectionAssert.AreEquivalent(new ColorTypes[] { ColorTypes.C }, card.Info.Colors);
  }

  [TestMethod]
  public void Serialize()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    JsonService.TrySerializeObject(card, out var serialized);

    Assert.IsNotNull(serialized);
  }

  [TestMethod]
  public void Deserialize()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    JsonService.TrySerializeObject(card, out var serialized);
    JsonService.TryDeserializeJson(serialized, out MTGCard deserialized);

    Assert.IsNotNull(deserialized);
    Assert.AreEqual(card.Info.Name, deserialized.Info.Name);
    Assert.AreEqual(card.Count, deserialized.Count);
  }

  [TestMethod("Card cound should be the given value if the value is more than one")]
  public void Count_MoreThanOne_IsValue()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var countValue = 10;

    card.Count = countValue;

    Assert.AreEqual(countValue, card.Count);
  }

  [TestMethod("Card cound should be the one if the value is less than one")]
  public void Count_LessThanOne_IsOne()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var countValue = -10;

    card.Count = countValue;

    Assert.AreEqual(1, card.Count);
  }
}