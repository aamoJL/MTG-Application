namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversibleCommand<T> : IReversibleCommand<T>
{
  public ReversibleCommand(T item) => Item = item;

  protected T Item { get; }

  public required IReversibleAction<T> ReversibleAction { get; set; }

  public void Execute() => ReversibleAction?.Action?.Invoke(Item);

  public void Undo() => ReversibleAction?.ReverseAction?.Invoke(Item);
}
