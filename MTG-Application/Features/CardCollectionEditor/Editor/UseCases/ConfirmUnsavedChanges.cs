using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollection.Editor.UseCases;

public partial class CardCollectionEditorViewModelCommands
{
  public class ConfirmUnsavedChanges(CardCollectionEditorViewModel viewmodel) : AsyncCommand<ISavable.ConfirmArgs>
  {
    protected override bool CanExecute(ISavable.ConfirmArgs? param) => param != null && !param.Cancelled && viewmodel.HasUnsavedChanges;

    protected override async Task Execute(ISavable.ConfirmArgs? param)
    {
      if (!CanExecute(param))
        return;

      switch (await viewmodel.Confirmers.CardCollectionConfirmers.SaveUnsavedChangesConfirmer
        .Confirm(CardCollectionConfirmers.GetSaveUnsavedChangesConfirmation(viewmodel.CollectionName)))
      {
        case ConfirmationResult.Yes:
          if (viewmodel.SaveCollectionCommand?.CanExecute(null) is true)
            await viewmodel.SaveCollectionCommand.ExecuteAsync(null);

          param!.Cancelled = viewmodel.HasUnsavedChanges;
          return;
        case ConfirmationResult.Cancel:
          param!.Cancelled = true;
          return;
      }
      ;
    }
  }
}