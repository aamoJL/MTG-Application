using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Databases.Repositories.DeckRepository;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.GeneralTests.Services.DatabaseTests.RepositoryTests.DeckRepositoryTests;

[TestClass]
public class DeleteDeckTests
{
  private readonly DeckRepositoryDependencies _dependencies = new();
  private readonly MTGCardDeck _savedDeck = new() { Name = "Saved Deck" };

  public DeleteDeckTests() => _dependencies.ContextFactory.Populate(new MTGCardDeckDTO(_savedDeck));

  [TestMethod("Should return true if the given deck was deleted")]
  public async Task Execute_WithDeck_Deleted_ReturnTrue()
  {
    var result = await new DeleteDeck(_dependencies.Repository).Execute(_savedDeck);

    Assert.IsTrue(result, "Should have returned true");
  }

  [TestMethod("Should return false if the given deck was not found")]
  public async Task Execute_WithDeck_NotFound_ReturnFalse()
  {
    var unsavedDeck = new MTGCardDeck() { Name = "Unsaved Deck" };
    var result = await new DeleteDeck(_dependencies.Repository).Execute(unsavedDeck);

    Assert.IsFalse(result, "Should have returned false");
  }

  [TestMethod("Should return true if the deck with the given name was deleted")]
  public async Task Execute_WithName_Deleted_ReturnTrue()
  {
    var result = await new DeleteDeck(_dependencies.Repository).Execute(_savedDeck.Name);

    Assert.IsTrue(result, "Should have returned true");
  }

  [TestMethod("Should return false if the deck with the given name was not found")]
  public async Task Execute_NotFound_ReturnFalse()
  {
    var result = await new DeleteDeck(_dependencies.Repository).Execute("Unsaved Deck");

    Assert.IsFalse(result, "Should have returned false");
  }

  [TestMethod("Should return false if the deck could not be deleted")]
  public async Task Execute_Failure_ReturnFalse()
  {
    _dependencies.Repository.DeleteFailure = true;

    var result = await new DeleteDeck(_dependencies.Repository).Execute(_savedDeck);

    Assert.IsFalse(result, "Should have returned false");
  }
}
