using MTGApplication.Features.EdhrecSearch.UseCases;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.EdhrecSearch.UseCases;

[TestClass]
public class FetchCardsByThemeTests
{
  [TestMethod]
  public async Task Fetch_CardsReturned()
  {
    var cards = new CardImportResult.Card[]
    {
      new(MTGCardInfoMocker.MockInfo()),
      new(MTGCardInfoMocker.MockInfo()),
      new(MTGCardInfoMocker.MockInfo()),
      new(MTGCardInfoMocker.MockInfo()),
    };
    var importer = new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success(cards)
    };
    var edhRecImporter = new TestEdhrecImporter()
    {
      CardNames = ["Name"]
    };

    var result = await new FetchCardsByTheme(importer, edhRecImporter).Execute(new("Theme", "Uri"));

    CollectionAssert.AreEqual(cards, result.Found);
  }
}
