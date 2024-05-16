using System.Collections.Generic;

namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversibleCollectionCommand<T> : IReversibleCommand<IEnumerable<T>>
{
  public ReversibleCollectionCommand(T item, IClassCopier<T> copier)
  {
    Copier = copier;
    Items = new List<T>(Copier.Copy(new T[] { item }));
  }

  public ReversibleCollectionCommand(IEnumerable<T> items, IClassCopier<T> copier)
  {
    Copier = copier;
    Items = new List<T>(Copier.Copy(items));
  }

  public ReversibleAction<IEnumerable<T>> ReversableAction { get; set; }

  private IEnumerable<T> Items { get; }
  private IClassCopier<T> Copier { get; }

  public void Execute() => ReversableAction?.Action?.Invoke(Copier.Copy(Items));

  public void Undo() => ReversableAction?.ReverseAction?.Invoke(Copier.Copy(Items));
}
