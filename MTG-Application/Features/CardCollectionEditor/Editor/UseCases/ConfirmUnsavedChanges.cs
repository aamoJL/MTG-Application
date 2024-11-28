using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.Editor.Services;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ConfirmUnsavedChanges(CardCollectionEditorViewModel viewmodel) : ViewModelAsyncCommand<CardCollectionEditorViewModel, ISavable.ConfirmArgs>(viewmodel)
  {
    protected override bool CanExecute(ISavable.ConfirmArgs? param) => param != null && !param.Cancelled && Viewmodel.HasUnsavedChanges;

    protected override async Task Execute(ISavable.ConfirmArgs? param)
    {
      if (!CanExecute(param))
        return;

      switch (await Viewmodel.Confirmers.SaveUnsavedChangesConfirmer
        .Confirm(CardCollectionEditorConfirmers.GetSaveUnsavedChangesConfirmation(Viewmodel.CardCollectionViewModel.Name)))
      {
        case ConfirmationResult.Yes:
          if (Viewmodel.CardCollectionViewModel.SaveCollectionCommand?.CanExecute(null) is true)
            await Viewmodel.CardCollectionViewModel.SaveCollectionCommand.ExecuteAsync(null);

          param!.Cancelled = Viewmodel.HasUnsavedChanges = Viewmodel.CardCollectionViewModel.HasUnsavedChanges;
          return;
        case ConfirmationResult.Cancel:
          param!.Cancelled = true;
          return;
      };
    }
  }
}