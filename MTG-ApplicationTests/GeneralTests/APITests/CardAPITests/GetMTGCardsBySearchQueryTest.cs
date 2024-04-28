using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.API.CardAPI;
using MTGApplication.General.Models.Card;

namespace MTGApplicationTests.General.APITests.CardAPITests;
[TestClass]
public partial class GetMTGCardsBySearchQueryTest
{
  private readonly ICardAPI<MTGCard> CardAPI = new ScryfallAPI(); // TODO: mock

  [TestMethod("Cards should be found with valid query")]
  public async Task Execute_WithValidQuery_CardsFound()
  {
    var query = "asd";

    var result = await new GetMTGCardsBySearchQuery(CardAPI).Execute(query);

    Assert.IsTrue(result.Found.Length > 0, "Cards were not found");
  }

  [TestMethod("Cards should not be found with empty query")]
  public async Task Execute_WithEmptyQuery_CardsNotFound()
  {
    var query = string.Empty;

    var result = await new GetMTGCardsBySearchQuery(CardAPI).Execute(query);

    Assert.AreEqual(0, result.TotalCount, "Cards should not have been found.");
  }

  [TestMethod("Worker should be busy when searching cards")]
  [ExpectedException(typeof(IsBusyException))]
  public async Task Execute_WithValidQuery_WorkerBusy()
  {
    var query = string.Empty;

    await new GetMTGCardsBySearchQuery(CardAPI)
    {
      Worker = new TestExceptionWorker()
    }.Execute(query);
  }
}
