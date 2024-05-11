using System.Collections.Generic;

namespace MTGApplication.General.Services.ReversibleCommandService;

public class ReversibleCommandStack
{
  private Stack<IReversibleCommand> UndoStack { get; } = new();
  private Stack<IReversibleCommand> RedoStack { get; } = new();

  public void Undo()
  {
    if (UndoStack.TryPop(out var command))
    {
      command.Undo();
      RedoStack.Push(command);
    }
  }

  public void Redo()
  {
    if (RedoStack.TryPop(out var command))
    {
      command.Execute();
      UndoStack.Push(command);
    }
  }

  public void PushAndExecute(IReversibleCommand command)
  {
    UndoStack.Push(command);
    RedoStack.Clear();
    command.Execute();
  }

  public void Clear()
  {
    UndoStack.Clear();
    RedoStack.Clear();
  }

  public bool CanUndo => UndoStack.Count > 0;

  public bool CanRedo => RedoStack.Count > 0;
}