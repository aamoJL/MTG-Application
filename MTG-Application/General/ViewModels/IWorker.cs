using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public interface IWorker
{
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
}
