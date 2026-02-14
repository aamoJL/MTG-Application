using MTGApplication.Features.CardCollectionEditor.UseCases;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplicationTests.TestUtility.Database;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.UseCases;

[TestClass]
public class SaveCardCollectionTests
{
  [TestMethod]
  public async Task Save_Success_Returns_True()
  {
    var result = await new SaveCardCollection(new TestRepository<MTGCardCollectionDTO>()
    {
      AddResult = (_) => Task.FromResult(true),
      ExistsResult = (_) => Task.FromResult(false),
    }).Execute(new(), "Name", false);

    Assert.IsTrue(result);
  }

  [TestMethod]
  public async Task Save_Exists_NoOverride_Returns_False()
  {
    var result = await new SaveCardCollection(new TestRepository<MTGCardCollectionDTO>()
    {
      AddResult = (_) => Task.FromResult(true),
      ExistsResult = (_) => Task.FromResult(true),
    }).Execute(new(), "Name", overrideOld: false);

    Assert.IsFalse(result);
  }

  [TestMethod]
  public async Task Save_Exists_Override_Returns_True()
  {
    var result = await new SaveCardCollection(new TestRepository<MTGCardCollectionDTO>()
    {
      AddResult = (_) => Task.FromResult(true),
      ExistsResult = (_) => Task.FromResult(true),
    }).Execute(new(), "Name", overrideOld: true);

    Assert.IsTrue(result);
  }

  [TestMethod]
  public async Task Save_Failed_Returns_False()
  {
    var result = await new SaveCardCollection(new TestRepository<MTGCardCollectionDTO>()
    {
      AddResult = (_) => Task.FromResult(false),
      ExistsResult = (_) => Task.FromResult(false),
    }).Execute(new(), "Name", overrideOld: false);

    Assert.IsFalse(result);
  }

  [TestMethod]
  public async Task Save_Success_Old_Deleted()
  {
    var deleted = false;
    var result = await new SaveCardCollection(new TestRepository<MTGCardCollectionDTO>()
    {
      ExistsResult = async (name) =>
      {
        if (name == "Old") return await Task.FromResult(true);
        else return await Task.FromResult(false);
      },
      AddResult = (_) => Task.FromResult(true),
      GetResult = (_) => Task.FromResult<MTGCardCollectionDTO?>(new MTGCardCollectionDTO("Name", [])),
      DeleteResult = async (_) =>
      {
        deleted = true;
        return await Task.FromResult(true);
      }
    }).Execute(new() { Name = "Old" }, "New", overrideOld: false);

    Assert.IsTrue(result);
    Assert.IsTrue(deleted);
  }
}
