using System;

namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversibleAction<T>
{
  public Action<T> Action => ActionMethod;
  public Action<T> ReverseAction => ReverseActionMethod;

  protected virtual void ActionMethod(T arg) { }
  protected virtual void ReverseActionMethod(T arg) { }
}