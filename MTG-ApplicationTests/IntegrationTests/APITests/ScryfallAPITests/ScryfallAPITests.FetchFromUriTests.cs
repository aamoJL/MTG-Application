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

      await Assert.ThrowsAsync<UriFormatException>(() => api.ImportWithUri(uri));
    }

    [TestMethod]
    public async Task Fetch_WithValidUri_CardsFound()
    {
      var api = new ScryfallAPI();
      var uri = "https://api.scryfall.com/cards/search?dir=asc&format=json&include_extras=false&include_multilingual=false&include_variations=false&order=released&page=2&q=set%3Aneo+unique%3Acards+order%3Areleased+direction%3Aasc+format%3Aany+game%3Apaper&unique=cards";

      var result = await api.ImportWithUri(uri);

      Assert.IsGreaterThan(0, result.TotalCount);
      Assert.IsNotEmpty(result.Found);
    }

    [TestMethod]
    public async Task Fetch_WithValidUri_Pagination_NextPageNotEmpty()
    {
      var api = new ScryfallAPI();
      var uri = "https://api.scryfall.com/cards/search?q=set:neo+order:Released+unique:Cards+direction:Asc+game:paper";

      var result = await api.ImportWithUri(uri);

      Assert.HasCount(api.PageSize, result.Found);
      Assert.IsGreaterThan(api.PageSize, result.TotalCount);
      Assert.AreNotEqual(string.Empty, result.NextPageUri);
    }

    [TestMethod]
    public async Task Fetch_WithValidUri_FetchAll_FoundCountSameAsTotalCount()
    {
      var api = new ScryfallAPI();
      var uri = "https://api.scryfall.com/cards/search?q=set:neo+order:Released+unique:Cards+direction:Asc+game:paper";

      var result = await api.ImportWithUri(uri, fetchAll: true);

      Assert.HasCount(result.TotalCount, result.Found);
      Assert.IsGreaterThan(api.PageSize, result.TotalCount);
      Assert.AreEqual(string.Empty, result.NextPageUri);
    }
  }
}
