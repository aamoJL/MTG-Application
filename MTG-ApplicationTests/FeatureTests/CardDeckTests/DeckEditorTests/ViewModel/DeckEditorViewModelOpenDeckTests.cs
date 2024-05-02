using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class DeckEditorViewModelOpenDeckTests : DeckEditorViewModelTestsBase
{
  [TestMethod("Should be able to execute with string, empty string or null")]
  public void OpenDeck_CanExecute()
  {
    var vm = MockVM();

    Assert.IsTrue(vm.OpenDeckCommand.CanExecute(null));
    Assert.IsTrue(vm.OpenDeckCommand.CanExecute(string.Empty));
    Assert.IsTrue(vm.OpenDeckCommand.CanExecute("Name"));
  }

  [TestMethod("Deck should be the same when loading a deck with an empty name")]
  public async Task OpenDeck_WithEmptyName_DeckIsSame()
  {
    var unsavedDeck = new MTGCardDeck() { Name = "Unsaved Deck" };
    var vm = MockVM(deck: unsavedDeck);

    await vm.OpenDeckCommand.ExecuteAsync(string.Empty);

    Assert.AreEqual(unsavedDeck, vm.Deck);
  }

  [TestMethod("Unsaved changes confirmation should be shown when loading a deck if there are unsaved changes")]
  public async Task OpenDeck_UnsavedChanges_UnsavedConfirmationShown()
  {
    var vm = MockVM(hasUnsavedChanges: true, confirmers: new()
    {
      SaveUnsavedChanges = new() { OnConfirm = (arg) => throw new ConfirmationException() }
    });

    await Assert.ThrowsExceptionAsync<ConfirmationException>(() => vm.OpenDeckCommand.ExecuteAsync(null));
  }

  [TestMethod("Load confirmation should be shown when loading a deck without a name")]
  public async Task OpenDeck_WithoutName_LoadConfirmationShown()
  {
    var vm = MockVM(confirmers: new()
    {
      LoadDeck = new() { OnConfirm = (arg) => throw new ConfirmationException() }
    });

    await Assert.ThrowsExceptionAsync<ConfirmationException>(() => vm.OpenDeckCommand.ExecuteAsync(null));
  }

  [TestMethod("Load confirmation should not be shown when loading a deck with a name")]
  public async Task OpenDeck_WithName_LoadConfirmationNotShown()
  {
    var vm = MockVM(confirmers: new()
    {
      LoadDeck = new() { OnConfirm = (arg) => throw new ConfirmationException() }
    });

    await vm.OpenDeckCommand.ExecuteAsync(_savedDeck.Name);
  }

  [TestMethod("Deck should be the loaded deck if the loading was successful")]
  public async Task OpenDeck_Loaded_DeckChanged()
  {
    var vm = MockVM(confirmers: new()
    {
      LoadDeck = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    });

    await vm.OpenDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(_savedDeck.Name, vm.Deck.Name);
  }

  [TestMethod("Dec should be the loaded deck if unsaved changes confirmation were not canceled")]
  public async Task OpenDeck_DontSaveChanges_DeckChanged()
  {
    var vm = MockVM(hasUnsavedChanges: true, confirmers: new()
    {
      SaveUnsavedChanges = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) },
      LoadDeck = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    });

    await vm.OpenDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(_savedDeck.Name, vm.Deck.Name);
  }

  [TestMethod("Deck should be the same if the loading fails when loading a deck")]
  public async Task OpenDeck_Failure_DeckIsSame()
  {
    _dependencies.Repository.GetFailure = true;

    var unsavedDeck = new MTGCardDeck() { Name = "Unsaved Deck" };
    var vm = MockVM(deck: unsavedDeck, confirmers: new()
    {
      LoadDeck = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    });

    await vm.OpenDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(unsavedDeck.Name, vm.Deck.Name);
  }

  [TestMethod("Deck should be the same if the loading was canceled when loading a deck")]
  public async Task OpenDeck_Canceled_DeckIsSame()
  {
    var unsavedDeck = new MTGCardDeck() { Name = "Unsaved Deck" };
    var vm = MockVM(deck: unsavedDeck, confirmers: new()
    {
      LoadDeck = new() { OnConfirm = (arg) => Task.FromResult<string?>(null) }
    });

    await vm.OpenDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(unsavedDeck.Name, vm.Deck.Name);
  }

  [TestMethod("Deck should be the same if the unsaved changes saving was canceled when loading a deck")]
  public async Task OpenDeck_ChangeSaveCanceled_DeckIsSame()
  {
    var unsavedDeck = new MTGCardDeck() { Name = "Unsaved Deck" };
    var vm = MockVM(deck: unsavedDeck, hasUnsavedChanges: true, confirmers: new()
    {
      SaveUnsavedChanges = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Cancel) },
      LoadDeck = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    });

    await vm.OpenDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(unsavedDeck.Name, vm.Deck.Name);
  }

  [TestMethod("ViewModel should be busy when loading a deck")]
  public async Task OpenDeck_Loading_IsBusy()
  {
    var vm = MockVM(confirmers: new()
    {
      LoadDeck = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    });

    await WorkerAssert.IsBusy(vm, () => vm.OpenDeckCommand.ExecuteAsync(null));
  }

  [TestMethod("Should have no unsaved changes if the deck was loaded")]
  public async Task OpenDeck_Loaded_NoUnsavedChanges()
  {
    var vm = MockVM(hasUnsavedChanges: true, confirmers: new()
    {
      SaveUnsavedChanges = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) },
      LoadDeck = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    });

    await vm.OpenDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(vm.HasUnsavedChanges);
  }

  [TestMethod("Success notification should be sent when deck has been loaded")]
  public async Task OpenDeck_Loaded_SuccessNotificationSent()
  {
    var vm = MockVM(hasUnsavedChanges: true, confirmers: new()
    {
      SaveUnsavedChanges = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) },
      LoadDeck = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    }, notifier: new()
    {
      OnNotify = (arg) => throw new NotificationException(arg.NotificationType)
    });

    await NotificationAssert.NotificationSent(NotificationType.Success,
      vm.OpenDeckCommand.ExecuteAsync(null));
  }

  [TestMethod("Error notification should be sent when there are failure on loading")]
  public async Task OpenDeck_Failure_ErrorNotificationSent()
  {
    _dependencies.Repository.GetFailure = true;

    var vm = MockVM(hasUnsavedChanges: true, confirmers: new()
    {
      SaveUnsavedChanges = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) },
      LoadDeck = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    }, notifier: new()
    {
      OnNotify = (arg) => throw new NotificationException(arg.NotificationType)
    });

    await NotificationAssert.NotificationSent(NotificationType.Error,
      vm.OpenDeckCommand.ExecuteAsync(null));
  }
}