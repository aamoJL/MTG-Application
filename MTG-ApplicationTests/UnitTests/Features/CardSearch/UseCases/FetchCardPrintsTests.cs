using MTGApplication.Features.CardSearch.UseCases;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.CardSearch.UseCases;

[TestClass]
public class FetchCardPrintsTests
{
  [TestMethod]
  public async Task Fetch_PrintsReturned()
  {
    var cards = new CardImportResult.Card[]
    {
      new(MTGCardInfoMocker.MockInfo()),
      new(MTGCardInfoMocker.MockInfo()),
      new(MTGCardInfoMocker.MockInfo()),
      new(MTGCardInfoMocker.MockInfo()),
    };
    var result = await new FetchCardPrints(new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success(cards)
    }).Execute("Uri");

    Assert.HasCount(4, result);
  }
}