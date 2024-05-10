using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplicationTests.Services;
using System.Text.Json;
using static MTGApplication.General.Models.Card.MTGCard;

namespace MTGApplicationTests.Models;

[TestClass]
public class MTGCardTests
{
  [TestMethod]
  public void GetSpellTypesTest()
  {
    var typeLine = "Artifact Creature — Phyrexian Construct";
    CollectionAssert.AreEquivalent(new[] { SpellType.Artifact, SpellType.Creature }, GetSpellTypes(typeLine));
  }

  [TestMethod]
  public void GetColorTypeNameTest()
  {
    var colorNames = ColorTypesExtensions.ColorNames;

    Assert.IsTrue(colorNames.Count > 0);

    foreach (var colorName in colorNames)
      Assert.AreEqual(colorName.Value, colorName.Key.GetFullName());
  }

  [TestMethod]
  public void GetColorTypeTest()
  {
    var frontFace = Mocker.MTGCardModelMocker.CreateCardFace(colors: [ColorTypes.W, ColorTypes.G], name: "WhiteGreen");
    var backFace = Mocker.MTGCardModelMocker.CreateCardFace(colors: [ColorTypes.R], name: "Red");

    CollectionAssert.AreEquivalent(new[] { ColorTypes.W, ColorTypes.G, ColorTypes.R }, GetColors(frontFace, backFace));
    CollectionAssert.AreEquivalent(new[] { ColorTypes.R }, GetColors(backFace, null));
  }

  [TestMethod]
  public void FromJSONTest()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var json = JsonSerializer.Serialize(card);

    var fromJson = JsonSerializer.Deserialize<MTGCard>(json);
    Assert.IsNotNull(fromJson);
    Assert.AreEqual(card.Info.Name, fromJson.Info.Name);
    Assert.AreEqual(card.Count, fromJson.Count);
  }
}