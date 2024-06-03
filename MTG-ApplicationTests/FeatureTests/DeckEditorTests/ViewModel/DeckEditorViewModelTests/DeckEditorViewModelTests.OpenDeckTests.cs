using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.CardDeck;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class OpenDeckTests : DeckEditorViewModelTestsBase,
    ICanExecuteWithParameterCommandTests, IUnsavedChangesCheckTests, IOpenCommandTests, IWorkerTests
  {
    [TestMethod]
    public void ValidParameter_CanExecute()
    {
      var vm = MockVM();

      Assert.IsTrue(vm.OpenDeckCommand.CanExecute(null));
      Assert.IsTrue(vm.OpenDeckCommand.CanExecute("Name"));
    }

    [TestMethod]
    public void InvalidParameter_CanNotExecute()
    {
      var vm = MockVM();

      Assert.IsFalse(vm.OpenDeckCommand.CanExecute(string.Empty));
    }

    [TestMethod]
    public async Task Execute_WithValidParameter_Executed()
    {
      var vm = MockVM(confirmers: new()
      {
        LoadDeckConfirmer = new TestExceptionConfirmer<string, string[]>()
      });

      await vm.OpenDeckCommand.ExecuteAsync(_savedDeck.Name);

      Assert.AreEqual(_savedDeck.Name, vm.DeckName);
    }

    [TestMethod("Load confirmation should not be shown when executing with a name parameter")]
    public async Task Open_WithParameter_NoLoadConfirmationShown()
    {
      var vm = MockVM(confirmers: new()
      {
        LoadDeckConfirmer = new TestExceptionConfirmer<string, string[]>()
      });

      await vm.OpenDeckCommand.ExecuteAsync(_savedDeck.Name);
    }

    [TestMethod("Deck should be the same when executing with an empty name parameter")]
    public async Task Execute_WithInvalidParameter_Canceled()
    {
      var unsavedDeck = new MTGCardDeck() { Name = "Unsaved Deck" };
      var vm = MockVM(deck: unsavedDeck);

      await vm.OpenDeckCommand.ExecuteAsync(string.Empty);

      Assert.AreEqual(unsavedDeck.Name, vm.DeckName);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_UnsavedChangesConfirmationShown()
    {
      var vm = MockVM(hasUnsavedChanges: true, confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
      });

      await ConfirmationAssert.ConfirmationShown(() => vm.OpenDeckCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Cancel_HasUnsavedChanges()
    {
      var unsavedDeck = new MTGCardDeck() { Name = "Unsaved Deck" };
      var vm = MockVM(deck: unsavedDeck, hasUnsavedChanges: true, confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Cancel) },
      });

      await vm.OpenDeckCommand.ExecuteAsync(null);

      Assert.IsTrue(vm.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Decline_NoUnsavedChanges()
    {
      var vm = MockVM(hasUnsavedChanges: true, confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) },
        LoadDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      });

      await vm.OpenDeckCommand.ExecuteAsync(null);

      Assert.IsFalse(vm.HasUnsavedChanges);
    }

    [TestMethod]
    public async Task Execute_HasUnsavedChanges_Accept_SaveConfirmationShown()
    {
      var vm = MockVM(deck: _savedDeck, hasUnsavedChanges: true, confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) },
        SaveDeckConfirmer = new TestExceptionConfirmer<string, string>()
      });

      await ConfirmationAssert.ConfirmationShown(() => vm.OpenDeckCommand.ExecuteAsync(null));
    }

    [TestMethod("Load confirmation should be shown when loading a deck without a name")]
    public async Task Open_OpenConfirmationShown()
    {
      var vm = MockVM(confirmers: new()
      {
        LoadDeckConfirmer = new TestExceptionConfirmer<string, string[]>()
      });

      await ConfirmationAssert.ConfirmationShown(() => vm.OpenDeckCommand.ExecuteAsync(null));
    }

    [TestMethod("Deck should be the same if the loading was canceled when loading a deck")]
    public async Task Open_Cancel_NoChanges()
    {
      var unsavedDeck = new MTGCardDeck() { Name = "Unsaved Deck" };
      var vm = MockVM(deck: unsavedDeck, confirmers: new()
      {
        LoadDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult<string?>(null) }
      });

      await vm.OpenDeckCommand.ExecuteAsync(null);

      Assert.AreEqual(unsavedDeck.Name, vm.DeckName);
    }

    [TestMethod("Deck should be the loaded deck if the loading was successful")]
    public async Task Open_Success_Changed()
    {
      var vm = MockVM(confirmers: new()
      {
        LoadDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      });

      await vm.OpenDeckCommand.ExecuteAsync(null);

      Assert.AreEqual(_savedDeck.Name, vm.DeckName);
    }

    [TestMethod("Success notification should be sent when deck has been loaded")]
    public async Task Open_Success_SuccessNotificationSent()
    {
      var vm = MockVM(hasUnsavedChanges: true, confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) },
        LoadDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      }, notifier: new()
      {
        OnNotify = (arg) => throw new NotificationException(arg)
      });

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => vm.OpenDeckCommand.ExecuteAsync(null));
    }

    [TestMethod("Error notification should be sent when there are failure on loading")]
    public async Task Open_Error_ErrorNotificationSent()
    {
      _dependencies.Repository.GetFailure = true;

      var vm = MockVM(hasUnsavedChanges: true, confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.No) },
        LoadDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      }, notifier: new()
      {
        OnNotify = (arg) => throw new NotificationException(arg)
      });

      await NotificationAssert.NotificationSent(NotificationType.Error,
        () => vm.OpenDeckCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task Execute_IsBusy()
    {
      var vm = MockVM(confirmers: new()
      {
        LoadDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      });

      await WorkerAssert.IsBusy(vm, () => vm.OpenDeckCommand.ExecuteAsync(null));
    }
  }
}
