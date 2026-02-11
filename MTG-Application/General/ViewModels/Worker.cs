using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public partial class Worker : INotifyPropertyChanged
{
  public Worker() => Tasks.CollectionChanged += Tasks_CollectionChanged;

  public bool IsBusy => Tasks.Count != 0;

  protected ObservableCollection<Task> Tasks { get; set; } = [];

  public event PropertyChangedEventHandler? PropertyChanged;

  /// <summary>
  /// Sets the <see cref="IsBusy"/> property to <see langword="true"/> for the duration of the <paramref name="task"/>
  /// </summary>
  public async Task<T> DoWork<T>(Task<T> task)
  {
    Tasks.Add(task);

    await Run(task, (task) =>
    {
      Tasks.Remove(task);

      if (task.Exception != null)
        throw task.Exception;
    });

    return task.Result;
  }

  /// <summary>
  /// Sets the <see cref="IsBusy"/> property to <see langword="true"/> for the duration of the <paramref name="task"/>
  /// </summary>
  public async Task DoWork(Task task)
  {
    Tasks.Add(task);

    await Run(task, (task) =>
    {
      Tasks.Remove(task);

      if (task.Exception != null)
        throw task.Exception;
    });
  }

  public async Task DoWork(Func<Task> task) => await DoWork(task());

  public async Task<T> DoWork<T>(Func<Task<T>> task) => await DoWork(task());

  private async Task Run(Task task, Action<Task> onFinished)
  {
    try
    {
      if (task.Status == TaskStatus.Created)
        task.Start();

      await task;
    }
    catch { throw; }
    finally
    {
      onFinished(task);
    }
  }

  private async Task Run<T>(Task<T> task, Action<Task<T>> onFinished)
  {
    try
    {
      if (task.Status == TaskStatus.Created)
        task.Start();

      await task;
    }
    catch { }
    finally
    {
      onFinished(task);
    }
  }

  private void Tasks_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    => PropertyChanged?.Invoke(this, new(nameof(IsBusy)));
}
