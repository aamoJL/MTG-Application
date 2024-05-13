using System;

namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversibleAction<T>
{
  public Action<T> Action { get; set; }
  public Action<T> ReverseAction { get; set; }
}