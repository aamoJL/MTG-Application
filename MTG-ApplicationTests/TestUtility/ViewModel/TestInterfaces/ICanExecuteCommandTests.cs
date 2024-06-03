namespace MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
internal interface ICanExecuteCommandTests
{
  void ValidState_CanExecute();
  void InvalidState_CanNotExecute();
}