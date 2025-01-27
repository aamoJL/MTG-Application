using MTGApplication.General.Models;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace MTGApplication.General.Services.ReversibleCommandService;

public partial class ReversibleCommandStack : INotifyCollectionChanged
{
  public ReversibleCommandStack()
    => UndoStack.CollectionChanged += UndoStack_CollectionChanged;

  private ObservableStack<IReversibleCommand> UndoStack { get; } = new();
  private Stack<IReversibleCommand> RedoStack { get; } = new();

  public CombinedReversibleCommand ActiveCombinedCommand { get; set; } = new();

  public event NotifyCollectionChangedEventHandler? CollectionChanged;

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

  public void PushAndExecuteActiveCombinedCommand()
  {
    if (ActiveCombinedCommand.CanExecute())
      PushAndExecute(ActiveCombinedCommand);

    ActiveCombinedCommand = new();
  }

  public void Clear()
  {
    UndoStack.Clear();
    RedoStack.Clear();
    ActiveCombinedCommand = new();
  }

  public bool CanUndo => UndoStack.Count > 0;

  public bool CanRedo => RedoStack.Count > 0;

  private void UndoStack_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    => CollectionChanged?.Invoke(this, e);
}