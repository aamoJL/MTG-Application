using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public interface IWorker
{
  public static IWorker Default => new DefaultWorker();
  
  public bool IsBusy { get; set; }

  /// <summary>
  /// Sets the <see cref="IsBusy"/> property to <see langword="true"/> for the duration of the <paramref name="task"/>
  /// </summary>
  public async Task<T> DoWork<T>(Task<T> task)
  {
    IsBusy = true;
    var result = await task;
    IsBusy = false;
    return result;
  }

  /// <summary>
  /// Sets the <see cref="IsBusy"/> property to <see langword="true"/> for the duration of the <paramref name="task"/>
  /// </summary>
  public async Task DoWork(Task task)
  {
    IsBusy = true;
    await task;
    IsBusy = false;
  }
}

public class DefaultWorker : IWorker
{
  public bool IsBusy { get; set; }
}
