namespace MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

internal interface IOpenCommandTests
{
  Task Open_OpenConfirmationShown();
  Task Open_Cancel_NoChanges();
  Task Open_Success_Changed();
  Task Open_Success_SuccessNotificationSent();
  Task Open_Error_ErrorNotificationSent();
}
