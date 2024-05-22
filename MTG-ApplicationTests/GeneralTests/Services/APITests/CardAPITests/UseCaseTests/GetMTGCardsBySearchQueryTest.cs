using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.GeneralTests.Services.APITests.CardAPITests.UseCaseTests;
[TestClass]
public partial class GetMTGCardsBySearchQueryTest
{
  private readonly DeckRepositoryDependencies _dependensies = new();

  [TestMethod("Cards should be found with a valid query")]
  public async Task Execute_WithValidQuery_CardsFound()
  {
    var query = "asd";
    _dependensies.CardAPI.ExpectedCards = [MTGCardModelMocker.CreateMTGCardModel(name: query)];

    var result = await new GetMTGCardsBySearchQuery(_dependensies.CardAPI).Execute(query);

    Assert.IsTrue(result.Found.Length > 0, "Cards were not found");
  }

  [TestMethod("Cards should not be found with an empty query")]
  public async Task Execute_WithEmptyQuery_CardsNotFound()
  {
    var query = string.Empty;
    _dependensies.CardAPI.ExpectedCards = [MTGCardModelMocker.CreateMTGCardModel(name: query)];

    var result = await new GetMTGCardsBySearchQuery(_dependensies.CardAPI).Execute(query);

    Assert.AreEqual(0, result.TotalCount, "Cards should not have been found.");
  }
}
