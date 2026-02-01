using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ConfirmNewCollection(CardCollectionEditorViewModel viewmodel) : AsyncCommand
  {
    protected override async Task Execute()
    {
      var unsavedArgs = new ISavable.ConfirmArgs();

      if (viewmodel.ConfirmUnsavedChangesCommand != null)
        await viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(unsavedArgs);

      if (unsavedArgs.Cancelled)
        return;

      await viewmodel.Worker.DoWork(New());
    }

    private async Task New() => await viewmodel.ChangeCollection(new());
  }
}