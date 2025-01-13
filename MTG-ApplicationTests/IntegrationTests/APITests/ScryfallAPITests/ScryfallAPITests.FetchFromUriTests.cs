using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.API.CardAPI;

namespace MTGApplicationTests.IntegrationTests.APITests.ScryfallAPITests;
public partial class ScryfallAPITests
{
  [TestClass]
  public class FetchFromUriTests
  {
    [TestMethod]
    public async Task Fetch_WithEmptyUri_ExceptionThrown()
    {
      var api = new ScryfallAPI();
      var uri = string.Empty;

      await Assert.ThrowsExceptionAsync<UriFormatException>(() => api.ImportWithUri(uri));
    }

    [TestMethod]
    public async Task Fetch_WithValidUri_CardsFound()
    {
      var api = new ScryfallAPI();
      var uri = "https://api.scryfall.com/cards/search?dir=asc&format=json&include_extras=false&include_multilingual=false&include_variations=false&order=released&page=2&q=set%3Aneo+unique%3Acards+order%3Areleased+direction%3Aasc+format%3Aany+game%3Apaper&unique=cards";

      var result = await api.ImportWithUri(uri);

      Assert.IsTrue(result.TotalCount > 0);
      Assert.IsTrue(result.Found.Length > 0);
    }

    [TestMethod]
    public async Task Fetch_WithValidUri_Pagination_NextPageNotEmpty()
    {
      var api = new ScryfallAPI();
      var uri = "https://api.scryfall.com/cards/search?q=set:neo+order:Released+unique:Cards+direction:Asc+game:paper";

      var result = await api.ImportWithUri(uri);

      Assert.AreEqual(api.PageSize, result.Found.Length);
      Assert.IsTrue(result.TotalCount > api.PageSize);
      Assert.IsTrue(result.NextPageUri != string.Empty);
    }

    [TestMethod]
    public async Task Fetch_WithValidUri_FetchAll_FoundCountSameAsTotalCount()
    {
      var api = new ScryfallAPI();
      var uri = "https://api.scryfall.com/cards/search?q=set:neo+order:Released+unique:Cards+direction:Asc+game:paper";

      var result = await api.ImportWithUri(uri, fetchAll: true);

      Assert.AreEqual(result.TotalCount, result.Found.Length);
      Assert.IsTrue(result.TotalCount > api.PageSize);
      Assert.AreEqual(string.Empty, result.NextPageUri);
    }
  }
}
