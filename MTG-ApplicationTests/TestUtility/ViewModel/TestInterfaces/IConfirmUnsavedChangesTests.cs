namespace MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

internal interface IConfirmUnsavedChangesTests
{
  Task NoUnsavedChanges_ReturnTrue();
  Task SaveCommandCanNotExecute_ReturnTrue();
  Task CanSave_UnsavedChangesConfirmationShown();
  Task CanSave_AcceptUnsavedSave_SaveConfirmationShown();
  Task CanSave_DeclineUnsavedSave_ReturnTrue();
  Task CanSave_CancelUnsavedSave_ReturnFalse();
  Task CanSave_AcceptUnsavedSave_CancelSave_ReturnFalse();
  Task CanSave_AcceptUnsavedSave_DeclineSave_ReturnFalse();
  Task CanSave_AcceptUnsavedSave_Save_ReturnTrue();
}
