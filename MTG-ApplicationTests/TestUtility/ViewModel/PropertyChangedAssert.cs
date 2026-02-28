using System.ComponentModel;

namespace MTGApplicationTests.TestUtility.ViewModel;

public static class PropertyChangedAssert
{
  public static void AssertPropertyChanged(this INotifyPropertyChanged self, string propertyName, Action action)
  {
    var changed = false;
    self.PropertyChanged += (_, e) =>
    {
      if (e.PropertyName == propertyName)
        changed = true;
    };

    action();

    if (!changed)
      throw new AssertFailedException($"Property [{propertyName}] did not change.");
  }

  public static async Task AssertPropertyChanged(this INotifyPropertyChanged self, string propertyName, Func<Task> action)
  {
    var changed = false;
    self.PropertyChanged += (_, e) =>
    {
      if (e.PropertyName == propertyName)
        changed = true;
    };

    await action();

    if (!changed)
      throw new AssertFailedException($"Property [{propertyName}] did not change.");
  }
}
