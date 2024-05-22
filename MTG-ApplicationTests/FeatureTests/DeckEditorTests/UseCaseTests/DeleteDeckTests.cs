using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility.Database;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.UseCaseTests;

[TestClass]
public class DeleteDeckTests
{
  private readonly DeckRepositoryDependencies _dependencies = new();
  private readonly MTGCardDeck _savedDeck = new() { Name = "Saved Deck" };

  public DeleteDeckTests() => _dependencies.ContextFactory.Populate(new MTGCardDeckDTO(_savedDeck));

  [TestMethod("Should return TRUE when the deck has been deleted")]
  public async Task Execute_Delete_ReturnTrue()
  {
    var result = await new DeleteDeck(_dependencies.Repository).Execute(_savedDeck);

    Assert.IsTrue(result);
  }

  [TestMethod("Should return FALSE when the deck could not be deleted")]
  public async Task Execute_Failure_ReturnFailure()
  {
    _dependencies.Repository.DeleteFailure = true;

    var result = await new DeleteDeck(_dependencies.Repository).Execute(_savedDeck);

    Assert.IsFalse(result);
  }

  [TestMethod("Should return FALSE if the deck does not exist in the repository")]
  public async Task Execute_DeckNotFound_ReturnFailure()
  {
    var result = await new DeleteDeck(_dependencies.Repository).Execute(new() { Name = "Unsaved Deck" });

    Assert.IsFalse(result);
  }
}