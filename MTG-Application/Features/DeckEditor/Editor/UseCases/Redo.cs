using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor;

public partial class DeckEditorViewModelCommands
{
  public class Redo(DeckEditorViewModel viewmodel) : ViewModelCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.UndoStack.CanRedo;

    protected override void Execute() => Viewmodel.UndoStack.Redo();
  }
}