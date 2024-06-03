namespace MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

internal interface ICanExecuteWithParameterCommandTests
{
  void ValidParameter_CanExecute();
  void InvalidParameter_CanNotExecute();
  Task Execute_WithValidParameter_Executed();
  Task Execute_WithInvalidParameter_Canceled();
}
