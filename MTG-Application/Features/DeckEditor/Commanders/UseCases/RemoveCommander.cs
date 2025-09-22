using MTGApplication.Features.DeckEditor.Commanders.UseCases.ReversibleCommands;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.ViewModels;
using static MTGApplication.Features.DeckEditor.Commanders.UseCases.ReversibleActions.CommanderViewModelReversibleActions;

namespace MTGApplication.Features.DeckEditor.Commanders.UseCases;

public partial class CommanderViewModelCommands
{
  public class RemoveCommander(CommanderViewModel viewmodel) : SyncCommand<DeckEditorMTGCard>
  {
    public CommanderViewModel Viewmodel { get; } = viewmodel;

    protected override bool CanExecute(DeckEditorMTGCard? param) => Viewmodel.Card != null;

    protected override void Execute(DeckEditorMTGCard? param)
    {
      if (!CanExecute(param)) return;

      Viewmodel.UndoStack.PushAndExecute(
        new ReversibleCommanderChangeCommand(null, Viewmodel.Card)
        {
          ReversibleAction = new ReversibleChangeCommanderAction(Viewmodel)
        });
    }
  }
}