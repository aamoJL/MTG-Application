using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public partial class DeckEditorViewModelCommands
{
  public class Redo(DeckEditorViewModel viewmodel) : ViewModelCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override bool CanExecute() => Viewmodel.UndoStack.CanRedo;

    protected override void Execute() => Viewmodel.UndoStack.Redo();
  }
}