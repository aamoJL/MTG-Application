using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;

namespace MTGApplication.Services;

/// <summary>
/// Service that handles commands that supports undo and redo
/// </summary>
public partial class CommandService
{
  // TODO: Limit undo stack?
  public Stack<ICommand> UndoCommandStack { get; } = new();
  public Stack<ICommand> RedoCommandStack { get; } = new();

  /// <summary>
  /// Undoes the last executed command.
  /// </summary>
  [RelayCommand]
  public void Undo()
  {
    if (UndoCommandStack.Count > 0)
    {
      var command = UndoCommandStack.Pop();
      command.Undo();
      RedoCommandStack.Push(command);
    }
  }

  /// <summary>
  /// Redoes the last undoed command
  /// </summary>
  [RelayCommand]
  public void Redo()
  {
    if (RedoCommandStack.Count > 0)
    {
      var command = RedoCommandStack.Pop();
      command.Execute();
      UndoCommandStack.Push(command);
    }
  }

  /// <summary>
  /// Adds the command to the undo stack, clears redo stack and executes the command.
  /// </summary>
  public void Execute(ICommand command)
  {
    UndoCommandStack.Push(command);
    RedoCommandStack.Clear();
    command.Execute();
  }

  /// <summary>
  /// Clears Undo and Redo stacks
  /// </summary>
  public void Clear()
  {
    UndoCommandStack.Clear();
    RedoCommandStack.Clear();
  }
}

public partial class CommandService
{
  /// <summary>
  /// Interface for <see cref="CommandService"/> commands
  /// </summary>
  public interface ICommand
  {
    public void Execute();
    public void Undo();
  }

  /// <summary>
  /// Command that executes multiple commands at once
  /// </summary>
  public class CombinedCommand : ICommand
  {
    private ICommand[] Commands { get; }

    public CombinedCommand(ICommand[] commands) => Commands = commands;

    public void Execute()
    {
      foreach (var command in Commands)
      {
        command.Execute();
      }
    }

    public void Undo()
    {
      foreach (var command in Commands)
      {
        command.Undo();
      }
    }
  }
}