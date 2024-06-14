using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.GeneralTests.ServiceTests.APITests.CardAPITests.UseCaseTests;
[TestClass]
public partial class GetMTGCardsBySearchQueryTest
{
  private readonly DeckRepositoryDependencies _dependensies = new();

  [TestMethod("Cards should be found with a valid query")]
  public async Task Execute_WithValidQuery_CardsFound()
  {
    var query = "asd";
    _dependensies.Importer.ExpectedCards = [new(MTGCardInfoMocker.MockInfo(name: query))];

    var result = await new FetchCardsWithSearchQuery(_dependensies.Importer).Execute(query);

    Assert.IsTrue(result.Found.Length > 0, "Cards were not found");
  }

  [TestMethod("Cards should not be found with an empty query")]
  public async Task Execute_WithEmptyQuery_CardsNotFound()
  {
    var query = string.Empty;
    _dependensies.Importer.ExpectedCards = [new(MTGCardInfoMocker.MockInfo(name: query))];

    var result = await new FetchCardsWithSearchQuery(_dependensies.Importer).Execute(query);

    Assert.AreEqual(0, result.TotalCount, "Cards should not have been found.");
  }
}
