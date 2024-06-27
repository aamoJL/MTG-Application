namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversiblePropertyChangeCommand<TModel, TValue>(TModel model, TValue oldValue, TValue newValue, IClassCopier<TModel> copier) : IReversibleCommand<(TModel Model, TValue Value)>
{
  public ReversibleAction<(TModel Model, TValue Value)> ReversibleAction { get; set; }

  public TModel Model { get; } = model;
  public TValue OldValue { get; } = oldValue;
  public TValue NewValue { get; } = newValue;
  public IClassCopier<TModel> Copier { get; } = copier;

  public void Execute() => ReversibleAction?.Action?.Invoke((Copier.Copy(Model), NewValue));
  public void Undo() => ReversibleAction?.ReverseAction?.Invoke((Copier.Copy(Model), OldValue));
}
