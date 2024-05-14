using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class DeckEditorViewModelSaveDeckTests : DeckEditorViewModelTestsBase
{
  [TestMethod("Should not be able to execute if the deck has no cards")]
  public void SaveDeck_NoCards_CanNotExecute()
  {
    var vm = MockVM();

    Assert.IsFalse(vm.SaveDeckCommand.CanExecute(null));
  }

  [TestMethod("Should be able to execute if the deck has cards")]
  public void SaveDeck_HasCards_CanExecute()
  {
    var vm = MockVM(deck: _savedDeck);

    Assert.IsTrue(vm.SaveDeckCommand.CanExecute(null));
  }

  [TestMethod("Should show save confirmation when saving the deck")]
  public async Task SaveDeck_ConfirmationShown()
  {
    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      SaveDeckConfirmer = new TestExceptionConfirmer<string, string>()
    });

    await ConfirmationAssert.ConfirmationShown(() => vm.SaveDeckCommand.ExecuteAsync(null));
  }

  [TestMethod("Should save the deck if a name was given to the confirmation")]
  public async Task SaveDeck_NameGiven_DeckSaved()
  {
    var newDeck = MTGCardDeckMocker.Mock("New Deck");
    var vm = MockVM(deck: newDeck, confirmers: new()
    {
      SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(newDeck.Name) }
    });

    await vm.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsTrue(await _dependencies.Repository.Exists(newDeck.Name));
  }

  [TestMethod("Should not save the deck if the given name is empty")]
  public async Task SaveDeck_EmptyName_DeckNotSaved()
  {
    var newDeck = MTGCardDeckMocker.Mock("New Deck");
    var vm = MockVM(deck: newDeck, confirmers: new()
    {
      SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(string.Empty) }
    });

    await vm.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(await _dependencies.Repository.Exists(newDeck.Name));
    Assert.IsFalse(await _dependencies.Repository.Exists(string.Empty));
  }

  [TestMethod("Should not save if the confirmation was canceled")]
  public async Task SaveDeck_Cancel_DeckNotSaved()
  {
    var newDeck = MTGCardDeckMocker.Mock("New Deck");
    var vm = MockVM(deck: newDeck, confirmers: new()
    {
      SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult<string?>(null) }
    });

    await vm.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(await _dependencies.Repository.Exists(newDeck.Name));
    Assert.IsFalse(await _dependencies.Repository.Exists(string.Empty));
  }

  [TestMethod("Should have no unsaved changes if the deck was saved")]
  public async Task SaveDeck_Saved_NoUnsavedChanges()
  {
    var vm = MockVM(deck: _savedDeck, hasUnsavedChanges: true, confirmers: new()
    {
      SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    });

    await vm.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(vm.HasUnsavedChanges);
  }

  [TestMethod("Unsaved changes state should not change if there was a failure when saving the deck")]
  public async Task SaveDeck_Failure_HasUnsavedChanges()
  {
    _dependencies.Repository.UpdateFailure = true;

    var vm = MockVM(deck: _savedDeck, hasUnsavedChanges: true, confirmers: new()
    {
      SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    });

    await vm.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsTrue(vm.HasUnsavedChanges);
  }

  [TestMethod("Decks name should be changed to the given name when the deck has been saved with a different name")]
  public async Task SaveDeck_Saved_NameChanged()
  {
    var newName = "New Name";
    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(newName) }
    });

    await vm.SaveDeckCommand.ExecuteAsync(null);

    Assert.AreEqual(newName, vm.DeckName);
  }

  [TestMethod("ViewModel should be busy when saving the deck")]
  public async Task SaveDeck_Saving_IsBusy()
  {
    var vm = MockVM(deck: _savedDeck, hasUnsavedChanges: true, confirmers: new()
    {
      SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    });

    await WorkerAssert.IsBusy(vm, () => vm.SaveDeckCommand.ExecuteAsync(null));
  }

  [TestMethod("Should show override confirmation when trying to save over an existing deck")]
  public async Task SaveDeck_Override_OverrideConfirmationShown()
  {
    var newDeck = MTGCardDeckMocker.Mock("New Deck");
    var vm = MockVM(deck: newDeck, confirmers: new()
    {
      SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) },
      OverrideDeckConfirmer = new() { OnConfirm = (arg) => throw new ConfirmationException() },
    });

    await Assert.ThrowsExceptionAsync<ConfirmationException>(() => vm.SaveDeckCommand.ExecuteAsync(null));
  }

  [TestMethod("Should not show override confirmation if saving with the same name")]
  public async Task SaveDeck_SavingExistingWithSameName_NoOverrideConfirmationShown()
  {
    var vm = MockVM(deck: _savedDeck, confirmers: new()
    {
      SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) },
      OverrideDeckConfirmer = new() { OnConfirm = (arg) => throw new ConfirmationException() },
    });

    await vm.SaveDeckCommand.ExecuteAsync(null);
  }

  [TestMethod("Existing deck should be overridden if override was accepted")]
  public async Task SaveDeck_Override_OldDeckIsNew()
  {
    var newDeck = MTGCardDeckMocker.Mock("New Deck");
    newDeck.Commander = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "New Commander");

    var vm = MockVM(deck: newDeck, confirmers: new()
    {
      SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) },
      OverrideDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) },
    });

    await vm.SaveDeckCommand.ExecuteAsync(null);

    var dbDeck = await _dependencies.Repository.Get(_savedDeck.Name);
    Assert.AreEqual(newDeck.Commander.Info.Name, dbDeck?.Commander?.Name);
  }

  [TestMethod("Saving should be canceled if overriding was canceled")]
  public async Task SaveDeck_CancelOverride_DeckNotSaved()
  {
    var newDeck = MTGCardDeckMocker.Mock("New Deck");
    newDeck.Commander = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "New Commander");

    var vm = MockVM(deck: newDeck, confirmers: new()
    {
      SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) },
      OverrideDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Cancel) },
    });

    await vm.SaveDeckCommand.ExecuteAsync(null);

    var dbDeck = await _dependencies.Repository.Get(_savedDeck.Name);
    Assert.AreEqual(_savedDeck.Commander?.Info.Name, dbDeck?.Commander?.Name);
  }

  [TestMethod("Success notification should be sent when the deck was saved")]
  public async Task SaveDeck_Saved_SuccessNotificationSent()
  {
    var vm = MockVM(deck: _savedDeck, hasUnsavedChanges: true, confirmers: new()
    {
      SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    }, notifier: new()
    {
      OnNotify = (arg) => throw new NotificationException(arg.NotificationType)
    });

    await NotificationAssert.NotificationSent(NotificationType.Success,
      () => vm.SaveDeckCommand.ExecuteAsync(null));
  }

  [TestMethod("Error notification should be sent when there are failure on saving")]
  public async Task SaveDeck_Saved_ErrorNotificationSent()
  {
    _dependencies.Repository.UpdateFailure = true;

    var vm = MockVM(deck: _savedDeck, hasUnsavedChanges: true, confirmers: new()
    {
      SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    }, notifier: new()
    {
      OnNotify = (arg) => throw new NotificationException(arg.NotificationType)
    });

    await NotificationAssert.NotificationSent(NotificationType.Error,
      () => vm.SaveDeckCommand.ExecuteAsync(null));
  }
}