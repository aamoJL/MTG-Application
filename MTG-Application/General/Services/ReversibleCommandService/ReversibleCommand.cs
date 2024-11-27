using System.Collections.Generic;

namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversibleCommand<T> : IReversibleCommand<T>
{
  public class DefaultCopier : IClassCopier<T>
  {
    public T Copy(T item) => item;
    public IEnumerable<T> Copy(IEnumerable<T> items) => items;
  }

  public ReversibleCommand(T item)
    => Item = item;

  public ReversibleCommand(T item, IClassCopier<T> copier)
  {
    Item = item;
    Copier = copier;
  }

  protected T Item { get; }
  protected IClassCopier<T> Copier { get; } = new DefaultCopier();

  public required ReversibleAction<T> ReversibleAction { get; set; }

  public void Execute() => ReversibleAction?.Action?.Invoke(Copier.Copy(Item));

  public void Undo() => ReversibleAction?.ReverseAction?.Invoke(Copier.Copy(Item));
}
