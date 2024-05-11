using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class DeckEditorViewModelSaveUnsavedChangesTests : DeckEditorViewModelTestsBase
{
  [TestMethod("Success notification should be sent when unsaved changes was saved")]
  public async Task SaveUnsaved_SameName_SuccessNotificationSent()
  {
    var vm = MockVM(deck: _savedDeck, hasUnsavedChanges: true, confirmers: new()
    {
      SaveUnsavedChanges = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) },
      SaveDeck = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    }, notifier: new()
    {
      OnNotify = (arg) => throw new NotificationException(arg.NotificationType)
    });

    await NotificationAssert.NotificationSent(NotificationType.Success,
      vm.NewDeckCommand.ExecuteAsync(null));
  }

  [TestMethod("Error notification should be sent when there are failure on saving unsaved changes")]
  public async Task SaveUnsaved_Failure_ErrorNotificationSent()
  {
    _dependencies.Repository.UpdateFailure = true;

    var vm = MockVM(deck: _savedDeck, hasUnsavedChanges: true, confirmers: new()
    {
      SaveUnsavedChanges = new() { OnConfirm = (arg) => Task.FromResult(ConfirmationResult.Yes) },
      SaveDeck = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    }, notifier: new()
    {
      OnNotify = (arg) => throw new NotificationException(arg.NotificationType)
    });

    await NotificationAssert.NotificationSent(NotificationType.Error,
      vm.NewDeckCommand.ExecuteAsync(null));
  }
}