using CommunityToolkit.Mvvm.ComponentModel;

namespace MTGApplicationTests.TestUtility.Mocker;

public partial class ObservableTestObject(int value) : ObservableObject
{
  [ObservableProperty] public partial int Value { get; set; } = value;
}