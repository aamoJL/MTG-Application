using MTGApplication.General.ViewModels;
using System.ComponentModel;

namespace MTGApplicationTests.TestUtility.ViewModel;

public class IsBusyException : UnitTestAssertException { }

public static class WorkerAssert
{
  public static async Task IsBusy(Worker worker, Func<Task> task)
  {
    var wasBusy = worker.IsBusy;

    if (worker is not INotifyPropertyChanged propertyNotifier)
      throw new AssertFailedException($"Worker does not implement {nameof(INotifyPropertyChanged)} interface");

    propertyNotifier.PropertyChanged += (s, e) =>
    {
      if (e.PropertyName == nameof(Worker.IsBusy) && s is Worker worker)
        wasBusy = wasBusy || worker.IsBusy;
    };

    await task.Invoke();

    Assert.IsTrue(wasBusy);
  }
}

