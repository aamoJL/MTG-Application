using System;

namespace MTGApplication.General.Services.ReversibleCommandService;

public interface IReversibleCommand
{
  public void Execute();
  public void Undo();
}

public interface IReversibleCommand<T> : IReversibleCommand
{
  ReversibleAction<T> ReversableAction { get; set; }
}