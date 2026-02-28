using MTGApplication.Features.CardSearch.UseCases;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.CardSearch.UseCases;

[TestClass]
public class FetchCardsTests
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
    var result = await new FetchCards(new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success(cards)
    }).Execute("query");

    CollectionAssert.AreEqual(cards, result.Found);
  }
}
