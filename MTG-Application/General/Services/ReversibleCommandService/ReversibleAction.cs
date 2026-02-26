using System;

namespace MTGApplication.General.Services.ReversibleCommandService;

public interface IReversibleAction<T>
{
  public Action<T> Action { get; }
  public Action<T> ReverseAction { get; }
}

public class ReversibleAction<T> : IReversibleAction<T>
{
  public Action<T> Action => ActionMethod;
  public Action<T> ReverseAction => ReverseActionMethod;

  protected virtual void ActionMethod(T arg) { }
  protected virtual void ReverseActionMethod(T arg) { }
}

public class ReversibleFunc<T> : IReversibleAction<T>
{
  public required Action<T> Action { get; init; }
  public required Action<T> ReverseAction { get; init; }
}