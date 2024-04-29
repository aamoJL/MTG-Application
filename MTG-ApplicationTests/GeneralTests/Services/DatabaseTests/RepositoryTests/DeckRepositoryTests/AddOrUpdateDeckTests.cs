using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Databases.Repositories.DeckRepository;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.GeneralTests.Services.DatabaseTests.RepositoryTests.DeckRepositoryTests;

[TestClass]
public class AddOrUpdateDeckTests
{
  private readonly RepositoryDependencies _dependencies = new();
  private readonly MTGCardDeck _savedDeck = new() { Name = "Saved Deck" };

  public AddOrUpdateDeckTests() => _dependencies.ContextFactory.Populate(new MTGCardDeckDTO(_savedDeck));

  [TestMethod("Should return true if the deck was added to the reposiotry")]
  public async Task Execute_Added_ReturnTrue()
  {
    var newDeck = new MTGCardDeck() { Name = "New Deck" };
    var result = await new AddOrUpdateDeck(_dependencies.Repository).Execute((newDeck, "Save Name"));

    Assert.IsTrue(result, "Should have returned true");
  }

  [TestMethod("Should return false if the deck could not be added to the repository")]
  public async Task Execute_AddFailure_ReturnFalse()
  {
    _dependencies.Repository.AddFailure = true;

    var newDeck = new MTGCardDeck() { Name = "New Deck" };
    var result = await new AddOrUpdateDeck(_dependencies.Repository).Execute((newDeck, "Save Name"));

    Assert.IsFalse(result, "Should have returned false");
  }

  [TestMethod("Should return true if an existing deck was updated")]
  public async Task Execute_Update_ReturnTrue()
  {
    var result = await new AddOrUpdateDeck(_dependencies.Repository).Execute((_savedDeck, _savedDeck.Name));

    Assert.IsTrue(result, "Should have returned true");
  }

  [TestMethod("Should return false if an existing deck could not be updated")]
  public async Task Execute_UpdateFailure_ReturnFalse()
  {
    _dependencies.Repository.UpdateFailure = true;
    var result = await new AddOrUpdateDeck(_dependencies.Repository).Execute((_savedDeck, _savedDeck.Name));

    Assert.IsFalse(result, "Should have returned false");
  }
}