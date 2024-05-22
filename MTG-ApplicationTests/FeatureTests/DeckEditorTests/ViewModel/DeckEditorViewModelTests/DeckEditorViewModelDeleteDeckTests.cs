using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

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

  [TestMethod("Deck should be deleted if the deletion was confirmed")]
  public async Task DeleteDeck_Accept_DeckDeleted()
  {
    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
    });

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(await _dependencies.Repository.Exists(_savedDeck.Name));
  }

  [TestMethod("Deck should reset when the deck has been deleted")]
  public async Task DeleteDeck_Accept_DeckReset()
  {
    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
    });

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(string.Empty, vm.DeckName);
  }

  [TestMethod("Deck should not reset if there are a failure when deleting the deck")]
  public async Task DeleteDeck_Failure_DeckNotReset()
  {
    _dependencies.Repository.DeleteFailure = true;

    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
    });

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(_savedDeck.Name, vm.DeckName);
  }

  [TestMethod("Deck should not be deleted if the deletion was canceled")]
  public async Task DeleteDeck_Cancel_DeckNotDeleted()
  {
    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Cancel) }
    });

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    Assert.IsTrue(await _dependencies.Repository.Exists(_savedDeck.Name));
  }

  [TestMethod("ViewModel should have no unsaved changes if the deck was deleted")]
  public async Task DeleteDeck_Deleted_NoUnsavedChanges()
  {
    var vm = MockVM(deck: _savedDeck, hasUnsavedChanges: true, confirmers: new()
    {
      DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
    });

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(vm.HasUnsavedChanges);
  }

  [TestMethod("Success notification should be sent when the deck was deleted")]
  public async Task DeleteDeck_Deleted_SuccessNotificationSent()
  {
    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
    }, notifier: new()
    {
      OnNotify = (arg) => throw new NotificationException(arg)
    });

    await NotificationAssert.NotificationSent(NotificationType.Success,
      () => vm.DeleteDeckCommand.ExecuteAsync(null));
  }

  [TestMethod("Error notification should be sent when there are failure on deletion")]
  public async Task DeleteDeck_Failure_ErrorNotificationSent()
  {
    _dependencies.Repository.DeleteFailure = true;

    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      DeleteDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) }
    }, notifier: new()
    {
      OnNotify = (arg) => throw new NotificationException(arg)
    });

    await NotificationAssert.NotificationSent(NotificationType.Error,
      () => vm.DeleteDeckCommand.ExecuteAsync(null));
  }

  [TestMethod]
  public async Task DeleteDeck_ConfirmationShown()
  {
    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      DeleteDeckConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
    });

    await ConfirmationAssert.ConfirmationShown(() => vm.DeleteDeckCommand.ExecuteAsync(null));
  }
}