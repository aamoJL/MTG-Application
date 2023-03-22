using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplicationTests.Services;
using System.Text.Json;
using static MTGApplication.Models.MTGCard;

namespace MTGApplicationTests.Models
{
  [TestClass]
  public class MTGCardTests
  {
    [TestMethod]
    public void GetSpellTypesTest()
    {
      string typeLine = "Artifact Creature — Phyrexian Construct";
      CollectionAssert.AreEquivalent(new[] {SpellType.Artifact, SpellType.Creature}, GetSpellTypes(typeLine));
    }

    [TestMethod]
    public void GetColorTypeNameTest()
    {
      CollectionAssert.AreEqual(new[] {"White", "Blue", "Black", "Red", "Green", "Multicolor", "Colorless"}, new[]
      {
        GetColorTypeName(ColorTypes.W),
        GetColorTypeName(ColorTypes.U),
        GetColorTypeName(ColorTypes.B),
        GetColorTypeName(ColorTypes.R),
        GetColorTypeName(ColorTypes.G),
        GetColorTypeName(ColorTypes.M),
        GetColorTypeName(ColorTypes.C),
      });
    }

    [TestMethod]
    public void GetColorTypeTest()
    {
      var frontFace = Mocker.MTGCardModelMocker.CreateCardFace(colors: new[] { ColorTypes.W, ColorTypes.G }, name: "WhiteGreen");
      var backFace = Mocker.MTGCardModelMocker.CreateCardFace(colors: new[] { ColorTypes.R }, name: "Red");

      CollectionAssert.AreEquivalent(new[] { ColorTypes.W, ColorTypes.G, ColorTypes.R }, GetColors(frontFace, backFace));
      CollectionAssert.AreEquivalent(new[] { ColorTypes.R }, GetColors(backFace, null));
    }

    [TestMethod]
    public void FromJSONTest()
    {
      var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
      var json = card.ToJSON();

      var fromJson = JsonSerializer.Deserialize<MTGCard>(json);
      Assert.IsNotNull(fromJson);
      Assert.AreEqual(card.Info.Name, fromJson.Info.Name);
      Assert.AreEqual(card.Count, fromJson.Count);
    }
  }
}
