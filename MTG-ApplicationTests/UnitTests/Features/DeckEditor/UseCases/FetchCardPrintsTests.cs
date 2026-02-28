using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases;

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
    };
    var importer = new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success(cards)
    };

    var result = await new FetchCardPrints(importer).Execute(new(MTGCardInfoMocker.MockInfo()));

    CollectionAssert.AreEqual(cards, result.Found);
  }
}
