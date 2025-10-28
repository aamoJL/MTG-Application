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

      Assert.IsEmpty(result.Found);
    }

    [TestMethod]
    public async Task Fetch_WithValidQuery_CardsFound()
    {
      var api = new ScryfallAPI();
      var query = "asd";

      var result = await api.ImportCardsWithSearchQuery(query);

      Assert.IsNotEmpty(result.Found);
      Assert.AreEqual(api.Name, result.Found[0].Info.ImporterName);
    }

    [TestMethod]
    public async Task Fetch_ReversibleCards_CardsFound()
    {
      var api = new ScryfallAPI();
      var query = "is:reversible";

      var result = await api.ImportCardsWithSearchQuery(query);

      Assert.IsNotEmpty(result.Found);
    }
  }
}
