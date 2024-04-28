using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardDeck;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;
[TestClass]
public class SaveUnsavedChangesTests
{
  private readonly UseCaseDependencies _dependencies = new();
  private readonly MTGCardDeck _deck = new() { Name = "Deck" };

  [TestMethod("Should show unsaved confirmation when executed")]
  [ExpectedException(typeof(ConfirmationException))]
  public async Task Execute_UnsavedConfirmShown()
  {
    await new SaveUnsavedChanges(_dependencies.Repository)
    {
      UnsavedChangesConfirmation = new TestExceptionConfirmer<ConfirmationResult>(),
    }.Execute(new(Deck: _deck));
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
    }.Execute(new(Deck: _deck));
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
    }.Execute(new(Deck: _deck));

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
    }.Execute(new(Deck: _deck));

    Assert.AreEqual(ConfirmationResult.Cancel, result);
  }
}