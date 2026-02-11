using MTGApplication.Features.CardCollectionEditor.UseCases;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplicationTests.TestUtility.Database;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.UseCases;

[TestClass]
public class FetchCardCollectionNamesTests
{
  [TestMethod]
  public async Task Fetch()
  {
    var items = new MTGCardCollectionDTO[]
    {
      new(name: "4", []),
      new(name: "1", []),
      new(name: "5", []),
      new(name: "2", []),
      new(name: "8", []),
    };

    var actual = await new FetchCardCollectionNames(new SimpleTestCardCollectionRepository()
    {
      GetAllResult = async () => [.. await Task.FromResult(items)]
    }).Execute();
    var expected = new string[] { "1", "2", "4", "5", "8", };

    CollectionAssert.AreEqual(expected, actual.ToList());
  }
}