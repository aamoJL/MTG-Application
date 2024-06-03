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
      var vm = MockVM(
        hasUnsavedChanges: true,
        deck: MTGCardDeckMocker.Mock("Deck"),
        confirmers: new()
        {
          SaveUnsavedChangesConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
        });

      await ConfirmationAssert.ConfirmationShown(() => vm.NewDeckCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Cancel_HasUnsavedChanges()
    {
      var vm = MockVM(
        hasUnsavedChanges: true,
        deck: MTGCardDeckMocker.Mock("Deck"),
        confirmers: new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Cancel) }
        });

      await vm.NewDeckCommand.ExecuteAsync(null);

      Assert.IsTrue(vm.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Decline_NoUnsavedChanges()
    {
      var vm = MockVM(
        hasUnsavedChanges: true,
        deck: MTGCardDeckMocker.Mock("Deck"),
        confirmers: new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.No) }
        });

      await vm.NewDeckCommand.ExecuteAsync(null);

      Assert.IsFalse(vm.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Accept_SaveConfirmationShown()
    {
      var vm = MockVM(
        hasUnsavedChanges: true,
        deck: MTGCardDeckMocker.Mock("Deck"),
        confirmers: new()
        {
          SaveUnsavedChangesConfirmer = new() { OnConfirm = async msg => await Task.FromResult(ConfirmationResult.Yes) },
          SaveDeckConfirmer = new TestExceptionConfirmer<string, string>()
        });

      await ConfirmationAssert.ConfirmationShown(() => vm.NewDeckCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Execute_Reset()
    {
      var vm = MockVM(deck: MTGCardDeckMocker.Mock("Deck"));

      await vm.NewDeckCommand.ExecuteAsync(null);

      Assert.AreEqual(string.Empty, vm.DeckName);
      Assert.AreEqual(null, vm.CommanderViewModel.Card);
      Assert.AreEqual(null, vm.PartnerViewModel.Card);
      Assert.AreEqual(0, vm.DeckCardList.Cards.Count);
    }
  }
}
