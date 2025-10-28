using MTGApplication.General.ViewModels;
using System.ComponentModel;

namespace MTGApplicationTests.TestUtility.ViewModel;

public class IsBusyException : UnitTestAssertException { }

public static class WorkerAssert
{
  public static async Task IsBusy(IWorker worker, Func<Task> task)
  {
    var wasBusy = worker.IsBusy;

    if (worker is not INotifyPropertyChanged propertyNotifier)
      throw new AssertFailedException($"Worker does not implement {nameof(INotifyPropertyChanged)} interface");

    propertyNotifier.PropertyChanged += (s, e) =>
    {
      if (e.PropertyName == nameof(IWorker.IsBusy) && s is IWorker worker)
        wasBusy = wasBusy || worker.IsBusy;
    };

    await task.Invoke();

    Assert.IsTrue(wasBusy);
  }
}

