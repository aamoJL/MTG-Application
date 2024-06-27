namespace MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

internal interface IUnsavedChangesCheckTests
{
  Task Execute_HasUnsavedChanges_UnsavedChangesConfirmationShown();
  Task Execute_HasUnsavedChanges_Cancel_HasUnsavedChanges();
  Task Execute_HasUnsavedChanges_Decline_NoUnsavedChanges();
  Task Execute_HasUnsavedChanges_Accept_SaveConfirmationShown();
}
