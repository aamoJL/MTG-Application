namespace MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

internal interface ISaveCommandTests
{
  Task Save_SaveConfirmationShown();
  Task Save_Cancel_NotSaved();
  Task Save_WithEmptyName_NotSaved();
  Task Save_NewName_Saved();
  Task Save_SameName_NoOverrideConfirmationShown();
  Task Save_Override_OverrideConfirmationShown();
  Task Save_Override_Cancel_NotSaved();
  Task Save_Override_Accept_Overridden();
  Task Save_Renamed_OldDeleted();
  Task Save_Renamed_NameChanged();
  Task Save_Success_NoUnsavedChanges();
  Task Save_Success_SuccessNotificationSent();
  Task Save_Error_ErrorNotificationSent();
}