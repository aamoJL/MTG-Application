namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversibleValueChangeCommand<T>(T newValue, T oldValue) : IReversibleCommand
{
  public required IReversibleAction<T> ReversibleAction { get; set; }

  public T NewValue { get; } = newValue;
  public T OldValue { get; } = oldValue;

  public void Execute() => ReversibleAction?.Action?.Invoke(NewValue);
  public void Undo() => ReversibleAction?.ReverseAction?.Invoke(OldValue);
}