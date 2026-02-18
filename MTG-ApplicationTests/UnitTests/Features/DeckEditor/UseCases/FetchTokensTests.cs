using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases;

[TestClass]
public class FetchTokensTests
{
  [TestMethod]
  public async Task Fetch_CardsReturned()
  {
    var importer = new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success([
        new(MTGCardInfoMocker.MockInfo()),
        new(MTGCardInfoMocker.MockInfo()),
        new(MTGCardInfoMocker.MockInfo()),
        ])
    };

    var result = await new FetchTokens(importer).Execute([]);

    Assert.HasCount(3, result.Found);
  }
}
