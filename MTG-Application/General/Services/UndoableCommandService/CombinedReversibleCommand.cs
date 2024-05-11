namespace MTGApplication.General.Services.ReversibleCommandService;

public class CombinedReversibleCommand : IReversibleCommand
{
  public CombinedReversibleCommand(IReversibleCommand[] commands) => Commands = commands;

  public IReversibleCommand[] Commands { get; }

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