namespace MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;
internal interface ICanExecuteCommandTests
{
  void ValidState_CanExecute();
  void InvalidState_CanNotExecute();
}

internal interface ICanExecuteCommandAsyncTests
{
  Task ValidState_CanExecute();
  Task InvalidState_CanNotExecute();
}