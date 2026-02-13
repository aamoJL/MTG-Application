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
    var result = await new FetchCardsByTheme(new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success(cards)
    }, new TestEdhrecImporter()
    {
      CardNames = ["Name"]
    }).Execute(new("Theme", "Uri"));

    CollectionAssert.AreEqual(cards, result.Found);
  }
}
