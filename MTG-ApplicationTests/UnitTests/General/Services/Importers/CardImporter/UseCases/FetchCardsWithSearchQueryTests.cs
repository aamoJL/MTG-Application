using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.General.Services.Importers.CardImporter.UseCases;

[TestClass]
public partial class FetchCardsWithSearchQueryTests
{
  private readonly DeckRepositoryDependencies _dependensies = new();

  [TestMethod(DisplayName = "Cards should be found with a valid query")]
  public async Task Execute_WithValidQuery_CardsFound()
  {
    var query = "asd";
    _dependensies.Importer.ExpectedCards = [new(MTGCardInfoMocker.MockInfo(name: query))];

    var result = await new FetchCardsWithSearchQuery(_dependensies.Importer).Execute(query);

    Assert.IsNotEmpty(result.Found, "Cards were not found");
  }

  [TestMethod(DisplayName = "Cards should not be found with an empty query")]
  public async Task Execute_WithEmptyQuery_CardsNotFound()
  {
    var query = string.Empty;
    _dependensies.Importer.ExpectedCards = [new(MTGCardInfoMocker.MockInfo(name: query))];

    var result = await new FetchCardsWithSearchQuery(_dependensies.Importer).Execute(query);

    Assert.AreEqual(0, result.TotalCount, "Cards should not have been found.");
  }
}
