﻿namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversiblePropertyChangeCommand<TModel, TValue>(TModel model, TValue oldValue, TValue newValue, IClassCopier<TModel>? copier = null) : IReversibleCommand<(TModel Model, TValue Value)>
{
  public TModel Model { get; } = model;
  public TValue OldValue { get; } = oldValue;
  public TValue NewValue { get; } = newValue;
  public IClassCopier<TModel> Copier { get; } = copier ?? new ReversibleCommand<TModel>.DefaultCopier();

  public required ReversibleAction<(TModel Model, TValue Value)> ReversibleAction { get; set; }

  public void Execute() => ReversibleAction?.Action?.Invoke((Copier.Copy(Model), NewValue));
  public void Undo() => ReversibleAction?.ReverseAction?.Invoke((Copier.Copy(Model), OldValue));
}

public class ReversiblePropertyChangeCommand<TValue>(TValue oldValue, TValue newValue) : IReversibleCommand<TValue>
{
  public TValue OldValue { get; } = oldValue;
  public TValue NewValue { get; } = newValue;

  public required ReversibleAction<TValue> ReversibleAction { get; set; }

  public void Execute() => ReversibleAction?.Action?.Invoke(NewValue);
  public void Undo() => ReversibleAction?.ReverseAction?.Invoke(OldValue);
}
