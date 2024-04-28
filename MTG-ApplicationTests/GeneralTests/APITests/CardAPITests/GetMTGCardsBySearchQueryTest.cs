using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.General.APITests.CardAPITests;
[TestClass]
public partial class GetMTGCardsBySearchQueryTest
{
  private readonly UseCaseDependencies _dependensies = new();

  [TestMethod("Cards should be found with a valid query")]
  public async Task Execute_WithValidQuery_CardsFound()
  {
    var query = "asd";
    _dependensies.CardAPI.ExpectedCards = [Mocker.MTGCardModelMocker.CreateMTGCardModel(name: query)];

    var result = await new GetMTGCardsBySearchQuery(_dependensies.CardAPI).Execute(query);

    Assert.IsTrue(result.Found.Length > 0, "Cards were not found");
  }

  [TestMethod("Cards should not be found with an empty query")]
  public async Task Execute_WithEmptyQuery_CardsNotFound()
  {
    var query = string.Empty;
    _dependensies.CardAPI.ExpectedCards = [Mocker.MTGCardModelMocker.CreateMTGCardModel(name: query)];

    var result = await new GetMTGCardsBySearchQuery(_dependensies.CardAPI).Execute(query);

    Assert.AreEqual(0, result.TotalCount, "Cards should not have been found.");
  }

  [TestMethod("Worker should be busy when searching cards")]
  [ExpectedException(typeof(IsBusyException))]
  public async Task Execute_WithValidQuery_WorkerBusy()
  {
    var query = string.Empty;
    _dependensies.CardAPI.ExpectedCards = [Mocker.MTGCardModelMocker.CreateMTGCardModel(name: query)];

    await new GetMTGCardsBySearchQuery(_dependensies.CardAPI)
    {
      Worker = new TestExceptionWorker()
    }.Execute(query);
  }
}
