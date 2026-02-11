using MTGApplication.Features.CardCollectionEditor.UseCases;
using MTGApplicationTests.TestUtility.Database;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.UseCases;

[TestClass]
public class DeleteCardCollectionTests
{
  [TestMethod]
  public async Task Delete_Failed_Return_False()
  {
    var result = await new DeleteCardCollection(new SimpleTestCardCollectionRepository()
    {
      DeleteResult = (_) => Task.FromResult(false)
    }).Execute(new());

    Assert.IsFalse(result);
  }

  [TestMethod]
  public async Task Delete_Success_Return_True()
  {
    var result = await new DeleteCardCollection(new SimpleTestCardCollectionRepository()
    {
      DeleteResult = (_) => Task.FromResult(true)
    }).Execute(new());

    Assert.IsTrue(result);
  }
}