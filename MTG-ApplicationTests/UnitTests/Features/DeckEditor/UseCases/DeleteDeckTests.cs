using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplicationTests.TestUtility.Database;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases;

[TestClass]
public class DeleteDeckTests
{
  [TestMethod]
  public async Task Delete_DeckDeleted()
  {
    var repo = new TestRepository<MTGCardDeckDTO>()
    {
      DeleteResult = _ => Task.FromResult(true)
    };

    var result = await new DeleteDeck(repo).Execute(new() { Name = "Deck" });

    Assert.IsTrue(result);
  }
}