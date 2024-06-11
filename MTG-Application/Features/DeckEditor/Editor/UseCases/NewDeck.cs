using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class DeckEditorViewModelCommands
{
  public class NewDeck(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel>(viewmodel)
  {
    protected override async Task Execute()
    {
      var unsavedArgs = new ISavable.ConfirmArgs();

      await new ConfirmUnsavedChanges(Viewmodel).Command.ExecuteAsync(unsavedArgs);

      if (unsavedArgs.Canceled)
        return;

      Viewmodel.Deck = new();
    }
  }
}