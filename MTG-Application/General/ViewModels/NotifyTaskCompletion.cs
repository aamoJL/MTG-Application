using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

// from: https://learn.microsoft.com/en-us/archive/msdn-magazine/2014/march/async-programming-patterns-for-asynchronous-mvvm-applications-data-binding
public sealed partial class NotifyTaskCompletion<TResult>(Task<TResult> task) : INotifyPropertyChanged
{
  public void Start() => _ = WatchTaskAsync(Task);

  private async Task WatchTaskAsync(Task task)
  {
    try
    {
      await task;
    }
    catch { }

    if (PropertyChanged is PropertyChangedEventHandler propertyChanged)
    {
      propertyChanged(this, new(nameof(Status)));
      propertyChanged(this, new(nameof(IsCompleted)));
      propertyChanged(this, new(nameof(IsNotCompleted)));

      if (task.IsCanceled)
        propertyChanged(this, new(nameof(IsCanceled)));
      else if (task.IsFaulted)
      {
        propertyChanged(this, new(nameof(IsFaulted)));
        propertyChanged(this, new(nameof(Exception)));
        propertyChanged(this, new(nameof(InnerException)));
        propertyChanged(this, new(nameof(ErrorMessage)));
      }
      else
      {
        propertyChanged(this, new(nameof(IsSuccessfullyCompleted)));
        propertyChanged(this, new(nameof(Result)));
      }
    }
  }

  public Task<TResult> Task { get; private set; } = task;

  public TResult? Result => (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : default;

  public TaskStatus Status => Task.Status;
  public bool IsCompleted => Task.IsCompleted;
  public bool IsNotCompleted => !Task.IsCompleted;
  public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;
  public bool IsCanceled => Task.IsCanceled;
  public bool IsFaulted => Task.IsFaulted;
  public AggregateException? Exception => Task.Exception;
  public Exception? InnerException => Exception?.InnerException;
  public string? ErrorMessage => InnerException?.Message;

  public event PropertyChangedEventHandler? PropertyChanged;
}