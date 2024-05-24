using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class SaveUnsavedChangesTests : DeckEditorViewModelTestsBase
  {
    [TestMethod("Success notification should be sent when unsaved changes was saved")]
    public async Task SaveUnsaved_SameName_SuccessNotificationSent()
    {
      var vm = MockVM(deck: _savedDeck, hasUnsavedChanges: true, confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) },
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      }, notifier: new()
      {
        OnNotify = (arg) => throw new NotificationException(arg)
      });

      await NotificationAssert.NotificationSent(NotificationType.Success,
        () => vm.NewDeckCommand.ExecuteAsync(null));
    }

    [TestMethod("Error notification should be sent when there are failure on saving unsaved changes")]
    public async Task SaveUnsaved_Failure_ErrorNotificationSent()
    {
      _dependencies.Repository.UpdateFailure = true;

      var vm = MockVM(deck: _savedDeck, hasUnsavedChanges: true, confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) },
        SaveDeckConfirmer = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
      }, notifier: new()
      {
        OnNotify = (arg) => throw new NotificationException(arg)
      });

      await NotificationAssert.NotificationSent(NotificationType.Error,
        () => vm.NewDeckCommand.ExecuteAsync(null));
    }

    [TestMethod]
    public async Task SaveUnsaved_Confirm_SaveConfirmationShown()
    {
      var vm = MockVM(deck: _savedDeck, hasUnsavedChanges: true, confirmers: new()
      {
        SaveUnsavedChangesConfirmer = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) },
        SaveDeckConfirmer = new TestExceptionConfirmer<string, string>()
      }, notifier: new()
      {
        OnNotify = (arg) => throw new NotificationException(arg)
      });

      await ConfirmationAssert.ConfirmationShown(() => vm.NewDeckCommand.ExecuteAsync(null));
    }
  }
}
