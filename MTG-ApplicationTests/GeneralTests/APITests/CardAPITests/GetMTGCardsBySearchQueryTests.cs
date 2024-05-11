using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.GeneralTests.APITests.CardAPITests;
[TestClass]
public class GetMTGCardsBySearchQueryTests
{
  // TODO: Move to integration tests

  [TestMethod]
  public async Task Execute_WithQuery_CardsFound()
  {
    var api = new TestCardAPI([Mocker.MTGCardModelMocker.CreateMTGCardModel()]);
    var task = new GetMTGCardsBySearchQuery(api);
    var query = "asd";

    var result = await task.Execute(query);

    CollectionAssert.AreEquivalent(api.ExpectedCards, result.Found);
  }

  [TestMethod]
  public async Task Execute_EmptyQuery_CardsNotFound()
  {
    var api = new TestCardAPI([Mocker.MTGCardModelMocker.CreateMTGCardModel()]);
    var task = new GetMTGCardsBySearchQuery(api);
    var query = string.Empty;

    var result = await task.Execute(query);

    Assert.AreEqual(0, result.Found.Length);
  }
}
