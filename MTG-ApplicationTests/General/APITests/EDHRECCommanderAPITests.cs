using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.API.CardAPI;
using MTGApplication.Models.Structs;

namespace MTGApplicationTests.API;

[TestClass]
public class EDHRECCommanderAPITests
{
  [TestMethod]
  public void GetUriTest_NoTheme()
  {
    var commanders = new Commanders("Ardenn, Intrepid Archaeologist", "Rograkh, Son of Rohgahh");
    var api = new EDHRECCommanderAPI();

    var result = api.GetUri(commanders);
    Assert.AreEqual(@"https://json.edhrec.com/pages/commanders/ardenn-intrepid-archaeologist-rograkh-son-of-rohgahh.json", result);
  }

  [TestMethod]
  public void GetUriTest_WithTheme()
  {
    var commanders = new Commanders("Ardenn, Intrepid Archaeologist", "Rograkh, Son of Rohgahh");
    var api = new EDHRECCommanderAPI();

    var result = api.GetUri(commanders, "/equipment");
    Assert.AreEqual(@"https://json.edhrec.com/pages/commanders/ardenn-intrepid-archaeologist-rograkh-son-of-rohgahh/equipment.json", result);
  }

  [TestMethod]
  public async Task GetThemesTest()
  {
    var commanders = new Commanders("Ardenn, Intrepid Archaeologist", "Rograkh, Son of Rohgahh");
    var api = new EDHRECCommanderAPI();
    var themes = await api.GetThemes(commanders);

    Assert.IsTrue(themes.Length > 0);
    Assert.IsTrue(Array.FindIndex(themes, x => x.Name == "Equipment") != -1);
  }

  [TestMethod]
  public async Task FetchNewCards_NoTheme()
  {
    var commanders = new Commanders("Ardenn, Intrepid Archaeologist", "Rograkh, Son of Rohgahh");
    var api = new EDHRECCommanderAPI();
    var uri = api.GetUri(commanders);

    var result = await api.FetchNewCards(uri);
    Assert.IsTrue(result.Length > 0); // NOTE: commanders might not have new cards, which will make the assert fail.
  }

  [TestMethod]
  public async Task FetchNewCards_WithTheme()
  {
    var commanders = new Commanders("Ardenn, Intrepid Archaeologist", "Rograkh, Son of Rohgahh");
    var api = new EDHRECCommanderAPI();
    var uri = api.GetUri(commanders, "/equipment");

    var result = await api.FetchNewCards(uri);
    Assert.IsTrue(result.Length > 0); // NOTE: commanders might not have new cards, which will make the assert fail.
  }
}
