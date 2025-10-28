using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplicationTests.TestUtility.Database;

namespace MTGApplicationTests.UnitTests.General.Services.Databases.Repositories.DeckRepositoryTests.UseCases;

[TestClass]
public class DeleteDeck
{
  private readonly DeckRepositoryDependencies _dependencies = new();
  private readonly MTGCardDeckDTO _savedDeck = new("Saved Deck");

  public DeleteDeck() => _dependencies.ContextFactory.Populate(_savedDeck);

  [TestMethod(DisplayName = "Should return true if the given deck was deleted")]
  public async Task Execute_WithDeck_Deleted_ReturnTrue()
  {
    var result = await new DeleteDeckDTO(_dependencies.Repository).Execute(_savedDeck);

    Assert.IsTrue(result, "Should have returned true");
  }

  [TestMethod(DisplayName = "Should return false if the given deck was not found")]
  public async Task Execute_WithDeck_NotFound_ReturnFalse()
  {
    var unsavedDeck = new MTGCardDeckDTO(name: "Unsaved Deck");
    var result = await new DeleteDeckDTO(_dependencies.Repository).Execute(unsavedDeck);

    Assert.IsFalse(result, "Should have returned false");
  }

  [TestMethod(DisplayName = "Should return true if the deck with the given name was deleted")]
  public async Task Execute_WithName_Deleted_ReturnTrue()
  {
    var result = await new DeleteDeckDTO(_dependencies.Repository).Execute(_savedDeck.Name);

    Assert.IsTrue(result, "Should have returned true");
  }

  [TestMethod(DisplayName = "Should return false if the deck with the given name was not found")]
  public async Task Execute_NotFound_ReturnFalse()
  {
    var result = await new DeleteDeckDTO(_dependencies.Repository).Execute("Unsaved Deck");

    Assert.IsFalse(result, "Should have returned false");
  }

  [TestMethod(DisplayName = "Should return false if the deck could not be deleted")]
  public async Task Execute_Failure_ReturnFalse()
  {
    _dependencies.Repository.DeleteFailure = true;

    var result = await new DeleteDeckDTO(_dependencies.Repository).Execute(_savedDeck);

    Assert.IsFalse(result, "Should have returned false");
  }
}
