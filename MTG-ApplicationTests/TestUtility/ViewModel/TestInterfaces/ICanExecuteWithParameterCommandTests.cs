namespace MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

internal interface ICanExecuteWithParameterCommandTests
{
  void ValidParameter_CanExecute();
  void InvalidParameter_CanNotExecute();
}

internal interface ICanExecuteWithParameterCommandAsyncTests
{
  Task ValidParameter_CanExecute();
  Task InvalidParameter_CanNotExecute();
}
