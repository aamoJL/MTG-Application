using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.GeneralTests.ServiceTests.DatabaseTests.RepositoryTests.DeckRepositoryTests.UseCaseTests;
[TestClass]
public class GetDeckTests
{
  private readonly DeckRepositoryDependencies _dependencies = new();
  private readonly MTGCardDeckDTO _savedDeck = MTGCardDeckDTOMocker.Mock("Saved Deck");

  public GetDeckTests() => _dependencies.ContextFactory.Populate(_savedDeck);

  [TestMethod("Should return saved deck with the given name")]
  public async Task Execute_Found_ReturnDeck()
  {
    var result = await new GetDeckDTO(_dependencies.Repository).Execute(_savedDeck.Name);

    Assert.AreEqual(result.Name, _savedDeck.Name);
  }

  [TestMethod("Should return null if the deck was not found")]
  public async Task Execute_NotFound_ReturnNull()
  {
    var result = await new GetDeckDTO(_dependencies.Repository).Execute("Unsaved Deck");

    Assert.IsNull(result);
  }
}
