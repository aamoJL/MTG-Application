using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class DeleteDeckTests
{
  private readonly DeckRepositoryDependencies _dependencies = new();
  private readonly MTGCardDeck _savedDeck = new() { Name = "Saved Deck" };

  public DeleteDeckTests() => _dependencies.ContextFactory.Populate(new MTGCardDeckDTO(_savedDeck));

  [TestMethod("Delete confirmation should be shown when executed")]
  [ExpectedException(typeof(ConfirmationException))]
  public async Task Execute_ConfirmationShown()
  {
    await new DeleteDeck(_dependencies.Repository)
    {
      DeleteConfirmation = new TestExceptionConfirmer<ConfirmationResult>()
    }.Execute(_savedDeck);
  }

  [TestMethod("Should return YES when the deck has been deleted")]
  public async Task Execute_Delete_ReturnYes()
  {
    var result = await new DeleteDeck(_dependencies.Repository)
    {
      DeleteConfirmation = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
    }.Execute(_savedDeck);

    Assert.AreEqual(ConfirmationResult.Yes, result);
  }

  [TestMethod("Should return CANCEL when the deletion has been canceled")]
  public async Task Execute_Canceled_ReturnCancel()
  {
    var result = await new DeleteDeck(_dependencies.Repository)
    {
      DeleteConfirmation = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Cancel) }
    }.Execute(_savedDeck);

    Assert.AreEqual(ConfirmationResult.Cancel, result);
  }

  [TestMethod("Should return FAILURE when the deck could not be deleted")]
  public async Task Execute_Failure_ReturnFailure()
  {
    _dependencies.Repository.DeleteFailure = true;

    var result = await new DeleteDeck(_dependencies.Repository)
    {
      DeleteConfirmation = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
    }.Execute(_savedDeck);

    Assert.AreEqual(ConfirmationResult.Failure, result);
  }

  [TestMethod("Should return FAILURE if the deck does not exist in the repository")]
  public async Task Execute_DeckNotFound_ReturnFailure()
  {
    var result = await new DeleteDeck(_dependencies.Repository)
    {
      DeleteConfirmation = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
    }.Execute(new() { Name = "Unsaved Deck" });

    Assert.AreEqual(ConfirmationResult.Failure, result);
  }
}