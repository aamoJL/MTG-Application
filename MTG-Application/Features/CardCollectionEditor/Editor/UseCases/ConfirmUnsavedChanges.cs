using MTGApplication.Features.CardCollection.Editor.ViewModels;
using MTGApplication.Features.CardCollectionEditor.CardCollection.Services;
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

      switch (await Viewmodel.Confirmers.CardCollectionConfirmers.SaveUnsavedChangesConfirmer
        .Confirm(CardCollectionConfirmers.GetSaveUnsavedChangesConfirmation(Viewmodel.CollectionName)))
      {
        case ConfirmationResult.Yes:
          if (Viewmodel.SaveCollectionCommand?.CanExecute(null) is true)
            await Viewmodel.SaveCollectionCommand.ExecuteAsync(null);

          param!.Cancelled = Viewmodel.HasUnsavedChanges;
          return;
        case ConfirmationResult.Cancel:
          param!.Cancelled = true;
          return;
      };
    }
  }
}