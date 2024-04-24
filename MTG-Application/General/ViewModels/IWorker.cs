using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public interface IWorker
{
  public bool IsBusy { get; set; }

  /// <summary>
  /// Sets the <see cref="IsBusy"/> property to <see langword="true"/> for the duration of the <paramref name="task"/>
  /// </summary>
  public abstract Task<T> DoWork<T>(Task<T> task);
}
