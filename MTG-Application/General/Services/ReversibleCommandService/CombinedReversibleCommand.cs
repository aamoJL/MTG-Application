using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.General.Services.ReversibleCommandService;

public class CombinedReversibleCommand : IReversibleCommand
{
  public List<IReversibleCommand> Commands { get; } = new();

  private bool Canceled { get; set; } = false;

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

  public void Cancel() => Canceled = true;

  public bool CanExecute() => Commands.Any() && !Canceled;
}