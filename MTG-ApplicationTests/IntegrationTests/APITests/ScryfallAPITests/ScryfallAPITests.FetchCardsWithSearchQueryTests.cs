using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.API.CardAPI;

namespace MTGApplicationTests.IntegrationTests.APITests.ScryfallAPITests;
public partial class ScryfallAPITests
{
  [TestClass]
  public class FetchCardsWithSearchQueryTests
  {
    [TestMethod]
    public async Task Fetch_WithEmptyQuery_NoCardsFound()
    {
      var api = new ScryfallAPI();
      var query = string.Empty;

      var result = await api.ImportCardsWithSearchQuery(query);

      Assert.AreEqual(0, result.Found.Length);
    }

    [TestMethod]
    public async Task Fetch_WithValidQuery_CardsFound()
    {
      var api = new ScryfallAPI();
      var query = "asd";

      var result = await api.ImportCardsWithSearchQuery(query);

      Assert.IsTrue(result.Found.Length > 0);
      Assert.AreEqual(api.Name, result.Found[0].APIName);
    }

    [TestMethod]
    public async Task Fetch_ReversibleCards_CardsFound()
    {
      var api = new ScryfallAPI();
      var query = "is:reversible";

      var result = await api.ImportCardsWithSearchQuery(query);

      Assert.IsTrue(result.Found.Length > 0);
    }
  }
}
