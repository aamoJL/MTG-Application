namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversibleCommand<T> : IReversibleCommand<T>
{
  public ReversibleCommand(T item, IClassCopier<T> copier)
  {
    Item = item;
    Copier = copier;
  }

  public ReversibleAction<T> ReversibleAction { get; set; }

  private T Item { get; }
  private IClassCopier<T> Copier { get; }

  public void Execute() => ReversibleAction?.Action?.Invoke(Copier.Copy(Item));
  public void Undo() => ReversibleAction?.ReverseAction?.Invoke(Copier.Copy(Item));
}
