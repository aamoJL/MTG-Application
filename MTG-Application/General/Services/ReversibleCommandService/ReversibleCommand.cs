namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversibleCommand<T>(T item, IClassCopier<T> copier) : IReversibleCommand<T>
{
  public ReversibleAction<T> ReversibleAction { get; set; }

  public void Execute() => ReversibleAction?.Action?.Invoke(copier.Copy(item));
  public void Undo() => ReversibleAction?.ReverseAction?.Invoke(copier.Copy(item));
}
