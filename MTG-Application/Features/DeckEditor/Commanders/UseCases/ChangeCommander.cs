using MTGApplication.Features.DeckEditor.Commanders.UseCases.ReversibleCommands;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class CommanderViewModelCommands
{
  public class ChangeCommander(CommanderCommands viewmodel) : ViewModelAsyncCommand<CommanderCommands, DeckEditorMTGCard>(viewmodel)
  {
    protected override async Task Execute(DeckEditorMTGCard card)
    {
      Viewmodel.UndoStack.PushAndExecute(new ReversibleCommanderChangeCommand(card, Viewmodel.GetCommander(), Viewmodel.CardCopier)
      {
        ReversibleAction = new CommanderViewModelReversibleActions.ReversibleChangeCommanderAction(Viewmodel)
      });

      await Task.Yield();
    }
  }
}