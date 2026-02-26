namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversiblePropertyChangeCommand<TModel, TValue>(TModel model, TValue oldValue, TValue newValue) : IReversibleCommand<(TModel Model, TValue Value)>
{
  public TModel Model { get; } = model;
  public TValue OldValue { get; } = oldValue;
  public TValue NewValue { get; } = newValue;

  public required IReversibleAction<(TModel Model, TValue Value)> ReversibleAction { get; set; }

  public void Execute() => ReversibleAction?.Action?.Invoke((Model, NewValue));
  public void Undo() => ReversibleAction?.ReverseAction?.Invoke((Model, OldValue));
}
