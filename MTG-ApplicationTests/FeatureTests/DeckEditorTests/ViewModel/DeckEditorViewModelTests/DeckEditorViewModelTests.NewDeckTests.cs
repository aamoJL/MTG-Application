using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class NewDeckTests : DeckEditorViewModelTestsBase,
    IUnsavedChangesCheckTests, INewCommandTests
  {
    [TestMethod("Unsaved changes confirmation should be shown when setting new deck if there are unsaved changes")]
    public async Task Execute_HasUnsavedChanges_UnsavedChangesConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = MTGCardDeckMocker.Mock("Deck"),
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
        }
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.NewDeckCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Cancel_HasUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = MTGCardDeckMocker.Mock("Deck"),
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel) }
        }
      }.MockVM();

      await viewmodel.NewDeckCommand.ExecuteAsync(null);

      Assert.IsTrue(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Decline_NoUnsavedChanges()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = MTGCardDeckMocker.Mock("Deck"),
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) }
        }
      }.MockVM();

      await viewmodel.NewDeckCommand.ExecuteAsync(null);

      Assert.IsFalse(viewmodel.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Accept_SaveConfirmationShown()
    {
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = MTGCardDeckMocker.Mock("Deck"),
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveDeckConfirmer = new TestExceptionConfirmer<string, string>()
        }
      }.MockVM();

      await ConfirmationAssert.ConfirmationShown(() => viewmodel.NewDeckCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task New_Success_Reset()
    {
      var viewmodel = new Mocker(_dependencies) { Deck = MTGCardDeckMocker.Mock("Deck") }.MockVM();

      await viewmodel.NewDeckCommand.ExecuteAsync(null);

      Assert.AreEqual(string.Empty, viewmodel.DeckName);
      Assert.AreEqual(null, viewmodel.CommanderViewModel.Card);
      Assert.AreEqual(null, viewmodel.PartnerViewModel.Card);
      Assert.AreEqual(0, viewmodel.DeckCardList.Cards.Count);
    }
  }
}
