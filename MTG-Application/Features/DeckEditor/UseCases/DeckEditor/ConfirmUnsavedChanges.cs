using MTGApplication.Features.DeckEditor.Services.DeckEditor;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class DeckEditorViewModel
{
  public class ConfirmUnsavedChanges(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel, ISavable.ConfirmArgs>(viewmodel)
  {
    protected override bool CanExecute(ISavable.ConfirmArgs param) => !param.Canceled;

    protected override async Task Execute(ISavable.ConfirmArgs param)
    {
      if (!CanExecute(param)) return;
      if (!Viewmodel.HasUnsavedChanges || !Viewmodel.SaveDeckCommand?.CanExecute(null) is true)
      {
        param.Canceled = true;
        return;
      }

      switch (await Viewmodel.Confirmers.SaveUnsavedChangesConfirmer
        .Confirm(DeckEditorConfirmers.GetSaveUnsavedChangesConfirmation(Viewmodel.DeckName)))
      {
        case ConfirmationResult.Yes:
          await Viewmodel.SaveDeckCommand.ExecuteAsync(null);
          param.Canceled = Viewmodel.HasUnsavedChanges;
          return;
        case ConfirmationResult.Cancel:
          param.Canceled = true;
          return;
      };
    }
  }
}