using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Commanders.UseCases;

public partial class CommanderViewModelCommands
{
  public class ChangeCommander(CommanderViewModel viewmodel) : AsyncCommand<DeckEditorMTGCard?>
  {
    public CommanderViewModel Viewmodel { get; } = viewmodel;

    protected override async Task Execute(DeckEditorMTGCard? card)
    {
      //Viewmodel.UndoStack.PushAndExecute(
      //  new ReversibleCommanderChangeCommand(card, Viewmodel.Card)
      //  {
      //    ReversibleAction = new ReversibleChangeCommanderAction(Viewmodel)
      //  });

      //await Task.Yield();
    }
  }
}