using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.UseCases;

public partial class CardCollectionPageViewModelCommands
{
  public class ConfirmNewCollection(CardCollectionEditorViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionEditorViewModel>(viewmodel)
  {
    protected override async Task Execute()
    {
      var unsavedArgs = new ISavable.ConfirmArgs();

      await Viewmodel.ConfirmUnsavedChangesCommand.ExecuteAsync(unsavedArgs);

      if (unsavedArgs.Cancelled) 
        return;

      Viewmodel.SelectedCardCollection = new();
      Viewmodel.HasUnsavedChanges = false;
    }
  }
}