using System.Collections.Generic;

namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversibleCollectionCommand<T> : IReversibleCommand<IEnumerable<T>>
{
  public ReversibleCollectionCommand(T item) => Items = new List<T>([item]);

  public ReversibleCollectionCommand(IEnumerable<T> items) => Items = [.. items];

  public required IReversibleAction<IEnumerable<T>> ReversibleAction { get; set; }

  private IEnumerable<T> Items { get; }

  public void Execute() => ReversibleAction?.Action?.Invoke(Items);

  public void Undo() => ReversibleAction?.ReverseAction?.Invoke(Items);
}