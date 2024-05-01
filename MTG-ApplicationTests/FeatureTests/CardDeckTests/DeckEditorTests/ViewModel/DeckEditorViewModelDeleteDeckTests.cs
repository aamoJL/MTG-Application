﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class DeckEditorViewModelDeleteDeckTests : DeckEditorViewModelTestsBase
{
  [TestMethod("Should be able to execute if the deck has a name")]
  public void DeleteDeck_HasName_CanExecute()
  {
    var vm = MockVM(deck: new() { Name = "New Deck" });

    Assert.IsTrue(vm.DeleteDeckCommand.CanExecute(null));
  }

  [TestMethod("Should not be able to execute if the deck has no name")]
  public void DeleteDeck_NoName_CanNotExecute()
  {
    var vm = MockVM(deck: new());

    Assert.IsFalse(vm.DeleteDeckCommand.CanExecute(null));
  }

  [TestMethod("Deck should be deleted if the deletion was accepted")]
  public async Task DeleteDeck_Accept_DeckDeleted()
  {
    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      DeleteDeck = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
    });

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(await _dependencies.Repository.Exists(_savedDeck.Name));
  }

  [TestMethod("Deck should reset when the deck has been deleted")]
  public async Task DeleteDeck_Accept_DeckReset()
  {
    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      DeleteDeck = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
    });

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.Deck.Name);
  }

  [TestMethod("Deck should not reset if there are a failure when deleting the deck")]
  public async Task DeleteDeck_Failure_DeckNotReset()
  {
    _dependencies.Repository.DeleteFailure = true;

    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      DeleteDeck = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
    });

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(_savedDeck.Name, vm.Deck.Name);
  }

  [TestMethod("Deck should not be deleted if the deletion was canceled")]
  public async Task DeleteDeck_Cancel_DeckNotDeleted()
  {
    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      DeleteDeck = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Cancel) }
    });

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    Assert.IsTrue(await _dependencies.Repository.Exists(_savedDeck.Name));
  }

  [TestMethod("ViewModel should have no unsaved changes if the deck was deleted")]
  public async Task DeleteDeck_Deleted_NoUnsavedChanges()
  {
    var vm = MockVM(deck: _savedDeck, hasUnsavedChanges: true, confirmers: new()
    {
      DeleteDeck = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
    });

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(vm.HasUnsavedChanges);
  }
}