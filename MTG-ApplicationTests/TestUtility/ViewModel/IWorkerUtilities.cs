using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.ViewModels;
using System.ComponentModel;

namespace MTGApplicationTests;

public class IsBusyException : UnitTestAssertException { }

public partial class TestExceptionWorker : ObservableObject, IWorker
{
  [ObservableProperty] private bool isBusy;

  public TestExceptionWorker() => PropertyChanged += TestExceptionWorker_PropertyChanged;

  private void TestExceptionWorker_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(IsBusy)) { throw new IsBusyException(); }
  }
}

public static class WorkerAssert
{
  public static async Task IsBusy(IWorker worker, Func<Task> task)
  {
    if (worker is not INotifyPropertyChanged propertyNotifier)
      throw new AssertFailedException($"Worker does not implement {nameof(INotifyPropertyChanged)} interface");

    propertyNotifier.PropertyChanged += WorkerPropertyNotifier_PropertyChanged;

    await Assert.ThrowsExceptionAsync<IsBusyException>(task);

    propertyNotifier.PropertyChanged -= WorkerPropertyNotifier_PropertyChanged;
  }

  private static void WorkerPropertyNotifier_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(IWorker.IsBusy)) throw new IsBusyException();
  }
}

