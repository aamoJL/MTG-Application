namespace MTGApplication.Features.DeckEditor._Commanders.UseCases;

//public partial class CommanderViewModelCommands
//{
//  public class RemoveCommander(CommanderViewModel viewmodel) : SyncCommand<DeckEditorMTGCard>
//  {
//    public CommanderViewModel Viewmodel { get; } = viewmodel;

//    protected override bool CanExecute(DeckEditorMTGCard? param)
//    {
//      return true;
//      //return Viewmodel.Card != null;
//    }

//    protected override void Execute(DeckEditorMTGCard? param)
//    {
//      //if (!CanExecute(param)) return;

//      //Viewmodel.UndoStack.PushAndExecute(
//      //  new ReversibleCommanderChangeCommand(null, Viewmodel.Card)
//      //  {
//      //    ReversibleAction = new ReversibleChangeCommanderAction(Viewmodel)
//      //  });
//    }
//  }
//}