namespace MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

internal interface IDeleteCommandTests
{
  Task Delete_DeleteConfirmationShown();
  Task Delete_Cancel_NotDeleted();
  Task Delete_Accept_Deleted();
  Task Delete_Success_SuccessNotificationSent();
  Task Delete_Error_ErrorNotificationSent();
}
