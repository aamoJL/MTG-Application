using CommunityToolkit.Mvvm.ComponentModel;

namespace MTGApplicationTests.TestUtility.Mocker;

public partial class ObservableTestObject(int value) : ObservableObject, IComparable
{
  [ObservableProperty] public partial int Value { get; set; } = value;

  public int CompareTo(object? obj)
  {
    if (obj is not ObservableTestObject other)
      return 0;

    if (Value > other.Value) return 1;
    if (Value < other.Value) return -1;
    else return 0;
  }
}