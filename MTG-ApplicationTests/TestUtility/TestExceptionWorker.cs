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

public static class IWorkerExtensions
{
  public static void ThrowWhenBusy(this IWorker value)
  {
    if (value is INotifyPropertyChanged propertyNotifier)
    {
      propertyNotifier.PropertyChanged += (s, e) =>
      {
        if (e.PropertyName == nameof(IWorker.IsBusy)) throw new IsBusyException();
      };
    }
    else throw new AssertFailedException($"Worker does not implement {nameof(INotifyPropertyChanged)} interface");
  }
}

