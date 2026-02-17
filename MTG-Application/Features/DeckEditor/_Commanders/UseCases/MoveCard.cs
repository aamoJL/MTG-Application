using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Commanders.UseCases;

public partial class CommanderViewModelCommands
{
  public class MoveCard
  {
    public class BeginMoveFrom(CommanderViewModel viewmodel) : SyncCommand<DeckEditorMTGCard>
    {
      public CommanderViewModel Viewmodel { get; } = viewmodel;

      protected override void Execute(DeckEditorMTGCard? _)
      {
        //Viewmodel.UndoStack.ActiveCombinedCommand.Commands.Add(
        //  new ReversibleCommanderChangeCommand(null, Viewmodel.Card)
        //  {
        //    ReversibleAction = new ReversibleChangeCommanderAction(Viewmodel)
        //  });
      }
    }

    public class BeginMoveTo(CommanderViewModel viewmodel) : AsyncCommand<DeckEditorMTGCard>
    {
      public CommanderViewModel Viewmodel { get; } = viewmodel;

      protected override async Task Execute(DeckEditorMTGCard? card)
      {
        //Viewmodel.UndoStack.ActiveCombinedCommand.Commands.Add(
        //  new ReversibleCommanderChangeCommand(card, Viewmodel.Card)
        //  {
        //    ReversibleAction = new ReversibleChangeCommanderAction(Viewmodel)
        //  });

        //await Task.Yield();
      }
    }

    public class ExecuteMove(CommanderViewModel viewmodel) : SyncCommand
    {
      public CommanderViewModel Viewmodel { get; } = viewmodel;

      protected override void Execute()
      {
        //Viewmodel.UndoStack.PushAndExecuteActiveCombinedCommand();
      }
    }
  }
}