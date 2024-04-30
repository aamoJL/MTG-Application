using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

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
      deck: await MTGCardDeckDTOMocker.Mock("Deck").AsMTGCardDeck(_dependencies.CardAPI),
      confirmers: new()
      {
        SaveUnsavedChanges = new() { OnConfirm = (arg) => throw new ConfirmationException() }
      });

    await vm.NewDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.Deck.Name);
    Assert.AreEqual(null, vm.Deck.Commander);
    Assert.AreEqual(null, vm.Deck.CommanderPartner);
    Assert.AreEqual(0, vm.Deck.DeckCards.Count);
  }

  [TestMethod("Unsaved changes confirmation should be shown when setting new deck if there are unsaved changes")]
  public async Task NewDeck_UnsavedChanges_UnsavedConfirmationShown()
  {
    var vm = MockVM(
      hasUnsavedChanges: true,
      deck: await MTGCardDeckDTOMocker.Mock("Deck").AsMTGCardDeck(_dependencies.CardAPI),
      confirmers: new()
      {
        SaveUnsavedChanges = new() { OnConfirm = (arg) => throw new ConfirmationException() }
      });

    await Assert.ThrowsExceptionAsync<ConfirmationException>(() => vm.NewDeckCommand.ExecuteAsync(null));
  }

  [TestMethod("Deck should be new when setting a new deck if unsaved changes will not be saved")]
  public async Task NewDeck_DontSaveUnsaved_DeckIsEmpty()
  {
    var vm = MockVM(
      hasUnsavedChanges: true,
      deck: await MTGCardDeckDTOMocker.Mock("Deck").AsMTGCardDeck(_dependencies.CardAPI),
      confirmers: new()
      {
        SaveUnsavedChanges = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) }
      });

    await vm.NewDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.Deck.Name);
    Assert.AreEqual(null, vm.Deck.Commander);
    Assert.AreEqual(null, vm.Deck.CommanderPartner);
    Assert.AreEqual(0, vm.Deck.DeckCards.Count);
  }

  [TestMethod("ViewModel should not have unsaved changes if the deck was reset")]
  public async Task NewDeck_DontSaveUnsaved_NoUnsavedChanges()
  {
    var vm = MockVM(
      hasUnsavedChanges: true,
      deck: await MTGCardDeckDTOMocker.Mock("Deck").AsMTGCardDeck(_dependencies.CardAPI),
      confirmers: new()
      {
        SaveUnsavedChanges = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) }
      });

    await vm.NewDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(vm.HasUnsavedChanges);
  }

  [TestMethod("Deck should be the same when canceling the unsaved changes confirmation when setting a new deck")]
  public async Task NewDeck_CancelSaveUnsaved_DeckIsSame()
  {
    var deck = await MTGCardDeckDTOMocker.Mock("Deck").AsMTGCardDeck(_dependencies.CardAPI);
    var vm = MockVM(
      hasUnsavedChanges: true,
      deck: deck,
      confirmers: new()
      {
        SaveUnsavedChanges = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Cancel) }
      });

    await vm.NewDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(deck, vm.Deck);
  }
}