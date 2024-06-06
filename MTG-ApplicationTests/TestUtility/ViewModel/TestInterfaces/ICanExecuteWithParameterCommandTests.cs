namespace MTGApplicationTests.TestUtility.ViewModel.TestInterfaces;

internal interface ICanExecuteWithParameterCommandTests
{
  void ValidParameter_CanExecute();
  void InvalidParameter_CanNotExecute();
}
