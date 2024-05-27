using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.API.CardAPI;

namespace MTGApplicationTests.IntegrationTests.APITests.ScryfallAPITests;
public partial class ScryfallAPITests
{
  [TestClass]
  public class FetchFromStringTests()
  {
    [TestMethod]
    public async Task Fetch_WithValidString_CardsFound()
    {
      var api = new ScryfallAPI();
      var importString = @"
              Black Lotus
              asd
              1 Blightstep Pathway
              4 Befriending the Moths // Imperial Moth
              _____ Bird Gets the Worm
              Katerina of Myra's Marvels
              Wear // Tear
              2 57e547cd-eba4-4a75-9c4e-8eeb6e00ddc4
              ";

      var result = await api.FetchFromString(importString);

      Assert.AreEqual(7, result.Found.Length);
      Assert.AreEqual(11, result.Found.Sum(x => x.Count));
    }

    [TestMethod]
    public async Task Fetch_WithEmptyString_NoCardsFound()
    {
      var api = new ScryfallAPI();
      var importString = string.Empty;

      var result = await api.FetchFromString(importString);

      Assert.AreEqual(0, result.Found.Length);
      Assert.AreEqual(0, result.NotFoundCount);
    }

    [TestMethod]
    public async Task Fetch_WithInvalidString_NotFoundCountIsCardCount()
    {
      var api = new ScryfallAPI();
      var importString = @"
            qiodhasoidha
            oashdpo aasjdpo
            2 aosdaposdpoad";

      var result = await api.FetchFromString(importString);

      Assert.AreEqual(0, result.Found.Length);
      Assert.AreEqual(3, result.NotFoundCount);
    }

    [TestMethod("Result should have found all valid items and not found count should be same as invalid item count" +
      "when fetching with partially invalid string")]
    public async Task Fetch_WithPartiallyInvalidString_SomeFoundSomeNotFound()
    {
      var api = new ScryfallAPI();
      var importString = @"
            Black Lotus
            oashdpo aasjdpo
            2 aosdaposdpoad";

      var result = await api.FetchFromString(importString);

      Assert.AreEqual(1, result.Found.Length);
      Assert.AreEqual(2, result.NotFoundCount);
    }
  }
}
