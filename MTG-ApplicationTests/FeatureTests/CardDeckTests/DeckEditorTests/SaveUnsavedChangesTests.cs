using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardDeck;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;
[TestClass]
public class SaveUnsavedChangesTests
{
  private readonly RepositoryDependencies _dependencies = new();
  private readonly MTGCardDeck _savedDeck = new() { Name = "Deck" };

  public SaveUnsavedChangesTests() => _dependencies.ContextFactory.Populate(new MTGCardDeckDTO(_savedDeck));

  [TestMethod("Should show unsaved confirmation when executed")]
  [ExpectedException(typeof(ConfirmationException))]
  public async Task Execute_UnsavedConfirmShown()
  {
    await new SaveUnsavedChanges(_dependencies.Repository)
    {
      UnsavedChangesConfirmation = new TestExceptionConfirmer<ConfirmationResult>(),
    }.Execute(_savedDeck);
  }

  [TestMethod("Should show save confirmation when accepted saving")]
  [ExpectedException(typeof(ConfirmationException))]
  public async Task Execute_Yes_SaveConfirmShown()
  {
    await new SaveUnsavedChanges(_dependencies.Repository)
    {
      UnsavedChangesConfirmation = new()
      {
        OnConfirm = async (arg) => await Task.FromResult(ConfirmationResult.Yes)
      },
      SaveConfirmation = new TestExceptionConfirmer<string, string>(),
    }.Execute(_savedDeck);
  }

  [TestMethod("Should return NO when declined saving")]
  public async Task Execute_No_ReturnNo()
  {
    var result = await new SaveUnsavedChanges(_dependencies.Repository)
    {
      UnsavedChangesConfirmation = new()
      {
        OnConfirm = async (arg) => await Task.FromResult(ConfirmationResult.No)
      },
    }.Execute(_savedDeck);

    Assert.AreEqual(ConfirmationResult.No, result);
  }

  [TestMethod("Should return CANCEL when canceling saving")]
  public async Task Execute_Cancel_ReturnCancel()
  {
    var result = await new SaveUnsavedChanges(_dependencies.Repository)
    {
      UnsavedChangesConfirmation = new()
      {
        OnConfirm = async (arg) => await Task.FromResult(ConfirmationResult.Cancel)
      },
    }.Execute(_savedDeck);

    Assert.AreEqual(ConfirmationResult.Cancel, result);
  }

  [TestMethod("Should return FAILURE when deck could not be saved")]
  public async Task Execute_Failure_ReturnFailure()
  {
    _dependencies.Repository.UpdateFailure = true;

    var result = await new SaveUnsavedChanges(_dependencies.Repository)
    {
      UnsavedChangesConfirmation = new()
      {
        OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes)
      },
      SaveConfirmation = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    }.Execute(_savedDeck);

    Assert.AreEqual(ConfirmationResult.Failure, result);
  }
}