using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ConfirmNewCollection(CardCollectionEditorViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionEditorViewModel>(viewmodel)
  {
    protected override async Task Execute()
    {
      var unsavedArgs = new ISavable.ConfirmArgs();

      if (Viewmodel.ConfirmUnsavedChangesCommand != null)
        await Viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(unsavedArgs);

      if (unsavedArgs.Cancelled)
        return;

      await Viewmodel.ChangeCollection(new());
      Viewmodel.HasUnsavedChanges = false;
    }
  }
}