using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.General.Services.Importers.CardImporter.UseCases;

[TestClass]
public partial class FetchCardsWithSearchQueryTests
{
  [TestMethod(DisplayName = "Cards should be found with a valid query")]
  public async Task Execute_WithValidQuery_CardsFound()
  {
    var query = "asd";
    var importer = new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo(name: query))])
    };

    var result = await new FetchCardsWithSearchQuery(importer).Execute(query);

    Assert.IsNotEmpty(result.Found, "Cards were not found");
  }
}
