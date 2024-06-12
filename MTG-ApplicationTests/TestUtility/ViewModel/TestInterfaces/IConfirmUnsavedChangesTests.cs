namespace MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

internal interface IConfirmUnsavedChangesTests
{
  Task NoUnsavedChanges_Success();
  Task ConfirmUnsavedChanges_UnsavedChangesConfirmationShown();
  Task ConfirmUnsavedChanges_Accept_SaveConfirmationShown();
  Task ConfirmUnsavedChanges_Decline_Success();
  Task ConfirmUnsavedChanges_Cancel_Canceled();
  Task ConfirmUnsavedChanges_Accept_CancelSave_Canceled();
  Task ConfirmUnsavedChanges_Accept_DeclineSave_Canceled();
  Task ConfirmUnsavedChanges_Accept_Save_Success();
}
