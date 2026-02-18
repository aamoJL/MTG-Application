using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Importers;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases;

[TestClass]
public class FetchDeckTests
{
  [TestMethod]
  public async Task Fetch_DeckReturned()
  {
    var repo = new TestRepository<MTGCardDeckDTO>()
    {
      GetResult = _ => Task.FromResult<MTGCardDeckDTO?>(new("Deck"))
    };
    var importer = new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success()
    };

    var result = await new FetchDeck(repo, importer).Execute("Deck");

    Assert.AreEqual("Deck", result.Name);
  }
}
