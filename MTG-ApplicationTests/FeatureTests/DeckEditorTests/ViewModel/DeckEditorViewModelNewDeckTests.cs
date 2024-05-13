using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class DeckEditorViewModelNewDeckTests : DeckEditorViewModelTestsBase
{
  [TestMethod("Should be able to execute with null")]
  public void NewDeck_CanExecute()
  {
    var vm = MockVM();
    Assert.IsTrue(vm.NewDeckCommand.CanExecute(null));
  }

  [TestMethod("Deck should be new when setting a new deck if there are no unsaved changes")]
  public async Task NewDeck_NoUnsavedChanges_DeckIsEmpty()
  {
    var vm = MockVM(
      deck: MTGCardDeckMocker.Mock("Deck"),
      confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => throw new ConfirmationException() }
      });

    await vm.NewDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.DeckName);
    Assert.AreEqual(null, vm.Commander);
    Assert.AreEqual(null, vm.Partner);
    Assert.AreEqual(0, vm.DeckCardList.Cards.Count);
  }

  [TestMethod("Unsaved changes confirmation should be shown when setting new deck if there are unsaved changes")]
  public async Task NewDeck_UnsavedChanges_UnsavedConfirmationShown()
  {
    var vm = MockVM(
      hasUnsavedChanges: true,
      deck: MTGCardDeckMocker.Mock("Deck"),
      confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => throw new ConfirmationException() }
      });

    await Assert.ThrowsExceptionAsync<ConfirmationException>(() => vm.NewDeckCommand.ExecuteAsync(null));
  }

  [TestMethod("Deck should be new when setting a new deck if unsaved changes will not be saved")]
  public async Task NewDeck_DontSaveUnsaved_DeckIsEmpty()
  {
    var vm = MockVM(
      hasUnsavedChanges: true,
      deck: MTGCardDeckMocker.Mock("Deck"),
      confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) }
      });

    await vm.NewDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.DeckName);
    Assert.AreEqual(null, vm.Commander);
    Assert.AreEqual(null, vm.Partner);
    Assert.AreEqual(0, vm.DeckCardList.Cards.Count);
  }

  [TestMethod("ViewModel should not have unsaved changes if the deck was reset")]
  public async Task NewDeck_DontSaveUnsaved_NoUnsavedChanges()
  {
    var vm = MockVM(
      hasUnsavedChanges: true,
      deck: MTGCardDeckMocker.Mock("Deck"),
      confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) }
      });

    await vm.NewDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(vm.HasUnsavedChanges);
  }

  [TestMethod("Deck should be the same when canceling the unsaved changes confirmation when setting a new deck")]
  public async Task NewDeck_CancelSaveUnsaved_DeckIsSame()
  {
    var deck = MTGCardDeckMocker.Mock("Deck");
    var vm = MockVM(
      hasUnsavedChanges: true,
      deck: deck,
      confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Cancel) }
      });

    await vm.NewDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(deck.Name, vm.DeckName);
  }
}