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
      var confirmer = new TestConfirmer<ConfirmationResult>();
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = MTGCardDeckMocker.Mock("Deck"),
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = confirmer
        }
      }.MockVM();

      await viewmodel.NewDeckCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
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
      var confirmer = new TestConfirmer<string, string>();
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = MTGCardDeckMocker.Mock("Deck"),
        HasUnsavedChanges = true,
        Confirmers = new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveDeckConfirmer = confirmer
        }
      }.MockVM();

      await viewmodel.NewDeckCommand.ExecuteAsync(null);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }

    [TestMethod]
    public async Task New_Success_Reset()
    {
      var viewmodel = new Mocker(_dependencies) { Deck = MTGCardDeckMocker.Mock("Deck") }.MockVM();

      await viewmodel.NewDeckCommand.ExecuteAsync(null);

      Assert.AreEqual(string.Empty, viewmodel.Name);
      Assert.IsNull(viewmodel.Commander.Card);
      Assert.IsNull(viewmodel.Partner.Card);
      Assert.AreEqual(0, viewmodel.DeckCardList.Cards.Count);
    }
  }
}
