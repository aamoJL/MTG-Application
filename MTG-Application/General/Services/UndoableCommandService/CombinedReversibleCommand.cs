using System.Collections.Generic;

namespace MTGApplication.General.Services.ReversibleCommandService;

public class CombinedReversibleCommand : IReversibleCommand
{
  public List<IReversibleCommand> Commands { get; } = new();

  public void Execute()
  {
    foreach (var command in Commands)
      command.Execute();
  }

  public void Undo()
  {
    foreach (var command in Commands)
      command.Undo();
  }
}