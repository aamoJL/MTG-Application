using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplicationTests.TestUtility.Database;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases;

[TestClass]
public class FetchDeckNamesTests
{
  [TestMethod]
  public async Task Fetch_NamesReturned_OrderedByName()
  {
    var repo = new TestRepository<MTGCardDeckDTO>()
    {
      GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>([
        new("Deck 3"),
        new("Deck 1"),
        new("Deck 2"),
      ])
    };

    var result = await new FetchDeckNames(repo).Execute();

    var expected = new string[] { "Deck 1", "Deck 2", "Deck 3" };

    CollectionAssert.AreEqual(expected, result.ToArray());
  }
}
