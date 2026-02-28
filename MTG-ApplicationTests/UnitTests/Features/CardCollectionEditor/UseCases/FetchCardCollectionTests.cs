using MTGApplication.Features.CardCollectionEditor.UseCases;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Importers;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.UseCases;

[TestClass]
public class FetchCardCollectionTests
{
  [TestMethod]
  public async Task Fetch_Success_Return_Value()
  {
    var actual = await new FetchCardCollection(
      repository: new TestRepository<MTGCardCollectionDTO>()
      {
        GetResult = async (_) => await Task.FromResult<MTGCardCollectionDTO>(new("Collection", []))
      },
      importer: new TestMTGCardImporter()
      ).Execute("Name");
    var expected = "Collection";

    Assert.AreEqual(expected, actual.Name);
  }

  [TestMethod]
  public async Task Fetch_Failure_Throw_Error()
  {
    await Assert.ThrowsExactlyAsync<KeyNotFoundException>(() => new FetchCardCollection(
      repository: new TestRepository<MTGCardCollectionDTO>()
      {
        GetResult = async (_) => await Task.FromResult<MTGCardCollectionDTO?>(null)
      },
      importer: new TestMTGCardImporter()
      ).Execute("Name"));
  }
}
