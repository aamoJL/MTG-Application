using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor;

public partial class DeckEditorViewModelCommands
{
  public class Undo(DeckEditorViewModel viewmodel) : ViewModelCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.UndoStack.CanUndo;

    protected override void Execute() => Viewmodel.UndoStack.Undo();
  }
}