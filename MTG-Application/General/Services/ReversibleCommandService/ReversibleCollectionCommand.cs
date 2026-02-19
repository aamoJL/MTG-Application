using System.Collections.Generic;

namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversibleCollectionCommand<T> : IReversibleCommand<IEnumerable<T>>
{
  public ReversibleCollectionCommand(T item) => Items = new List<T>([item]);

  public ReversibleCollectionCommand(IEnumerable<T> items) => Items = [.. items];

  public ReversibleCollectionCommand(T item, IClassCopier<T> copier)
  {
    Copier = copier;
    Items = [.. Copier.Copy([item])];
  }

  public ReversibleCollectionCommand(IEnumerable<T> items, IClassCopier<T> copier)
  {
    Copier = copier;
    Items = [.. Copier.Copy(items)];
  }

  public required ReversibleAction<IEnumerable<T>> ReversibleAction { get; set; }

  private IEnumerable<T> Items { get; }
  private IClassCopier<T> Copier { get; } = new ReversibleCommand<T>.DefaultCopier();

  public void Execute() => ReversibleAction?.Action?.Invoke(Copier.Copy(Items));

  public void Undo() => ReversibleAction?.ReverseAction?.Invoke(Copier.Copy(Items));
}