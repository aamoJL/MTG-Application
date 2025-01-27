using MTGApplication.Features.DeckEditor.Commanders.UseCases.ReversibleCommands;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.Commanders.UseCases.ReversibleActions.CommanderViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.Commanders.UseCases;

public partial class CommanderViewModelCommands
{
  public class ChangeCommander(CommanderViewModel viewmodel) : ViewModelAsyncCommand<CommanderViewModel, DeckEditorMTGCard?>(viewmodel)
  {
    protected override async Task Execute(DeckEditorMTGCard? card)
    {
      Viewmodel.UndoStack.PushAndExecute(
        new ReversibleCommanderChangeCommand(card, Viewmodel.Card)
        {
          ReversibleAction = new ReversibleChangeCommanderAction(Viewmodel)
        });

      await Task.Yield();
    }
  }
}