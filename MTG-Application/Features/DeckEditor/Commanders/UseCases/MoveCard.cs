using MTGApplication.Features.DeckEditor.Commanders.UseCases.ReversibleCommands;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class CommanderViewModelCommands
{
  public class MoveCard
  {
    public class BeginMoveFrom(CommanderViewModel viewmodel) : ViewModelCommand<CommanderViewModel, DeckEditorMTGCard>(viewmodel)
    {
      protected override void Execute(DeckEditorMTGCard _)
      {
        Viewmodel.UndoStack.ActiveCombinedCommand.Commands.Add(
          new ReversibleCommanderChangeCommand(null, Viewmodel.Card, Viewmodel.CardCopier)
          {
            ReversibleAction = new CommanderViewModelReversibleActions.ReversibleChangeCommanderAction(Viewmodel)
          });
      }
    }

    public class BeginMoveTo(CommanderViewModel viewmodel) : ViewModelAsyncCommand<CommanderViewModel, DeckEditorMTGCard>(viewmodel)
    {
      protected override async Task Execute(DeckEditorMTGCard card)
      {
        Viewmodel.UndoStack.ActiveCombinedCommand.Commands.Add(new ReversibleCommanderChangeCommand(card, Viewmodel.Card, Viewmodel.CardCopier)
        {
          ReversibleAction = new CommanderViewModelReversibleActions.ReversibleChangeCommanderAction(Viewmodel)
        });
        await Task.Yield();
      }
    }

    public class ExecuteMove(CommanderViewModel viewmodel) : ViewModelCommand<CommanderViewModel, DeckEditorMTGCard>(viewmodel)
    {
      protected override void Execute(DeckEditorMTGCard _)
        => Viewmodel.UndoStack.PushAndExecuteActiveCombinedCommand();
    }
  }
}