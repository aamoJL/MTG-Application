using MTGApplication.Features.DeckEditor.Commanders.UseCases.ReversibleCommands;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.Commanders.UseCases.ReversibleActions.CommanderViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.Commanders.UseCases;

public partial class CommanderViewModelCommands
{
  public class MoveCard
  {
    public class BeginMoveFrom(CommanderCommands viewmodel) : ViewModelCommand<CommanderCommands, DeckEditorMTGCard>(viewmodel)
    {
      protected override void Execute(DeckEditorMTGCard? _)
      {
        Viewmodel.UndoStack.ActiveCombinedCommand.Commands.Add(
          new ReversibleCommanderChangeCommand(null, Viewmodel.GetCommander(), Viewmodel.CardCopier)
          {
            ReversibleAction = new ReversibleChangeCommanderAction(Viewmodel)
          });
      }
    }

    public class BeginMoveTo(CommanderCommands viewmodel) : ViewModelAsyncCommand<CommanderCommands, DeckEditorMTGCard>(viewmodel)
    {
      protected override async Task Execute(DeckEditorMTGCard? card)
      {
        Viewmodel.UndoStack.ActiveCombinedCommand.Commands.Add(new ReversibleCommanderChangeCommand(card, Viewmodel.GetCommander(), Viewmodel.CardCopier)
        {
          ReversibleAction = new ReversibleChangeCommanderAction(Viewmodel)
        });
        await Task.Yield();
      }
    }

    public class ExecuteMove(CommanderCommands viewmodel) : ViewModelCommand<CommanderCommands, DeckEditorMTGCard>(viewmodel)
    {
      protected override void Execute(DeckEditorMTGCard? _)
        => Viewmodel.UndoStack.PushAndExecuteActiveCombinedCommand();
    }
  }
}