using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.UseCases;

public partial class DeckEditorViewModelCommands
{
  public class NewDeck(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override async Task Execute()
    {
      var unsavedArgs = new ISavable.ConfirmArgs();

      await new ConfirmUnsavedChanges(Viewmodel).Command.ExecuteAsync(unsavedArgs);

      if (unsavedArgs.Cancelled)
        return;

      Viewmodel.Deck = new();
      Viewmodel.UndoStack.Clear();
      Viewmodel.HasUnsavedChanges = false;
    }
  }
}