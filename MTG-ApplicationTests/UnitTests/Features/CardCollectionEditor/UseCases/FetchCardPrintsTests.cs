using MTGApplication.Features.CardCollectionEditor.UseCases;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.UseCases;

[TestClass]
public class FetchCardPrintsTests
{
  [TestMethod]
  public async Task Fetch()
  {
    var cards = new CardImportResult.Card[] {
      new(MTGCardInfoMocker.MockInfo()),
      new(MTGCardInfoMocker.MockInfo()),
      new(MTGCardInfoMocker.MockInfo()),
    };

    var result = await new FetchCardPrints(new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success(cards)
    }).Execute("uri");

    CollectionAssert.AreEquivalent(result.Found, cards);
  }
}
