using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public partial class DeckEditorViewModelCommands
{
  public class ConfirmUnsavedChanges(DeckEditorViewModel viewmodel) : ViewModelAsyncCommand<DeckEditorViewModel, ISavable.ConfirmArgs>(viewmodel)
  {
    protected override bool CanExecute(ISavable.ConfirmArgs param) => !param.Cancelled && Viewmodel.HasUnsavedChanges;

    protected override async Task Execute(ISavable.ConfirmArgs param)
    {
      if (!CanExecute(param)) return;

      switch (await Viewmodel.Confirmers.SaveUnsavedChangesConfirmer
        .Confirm(DeckEditorConfirmers.GetSaveUnsavedChangesConfirmation(Viewmodel.Name)))
      {
        case ConfirmationResult.Yes:
          await Viewmodel.SaveDeckCommand.ExecuteAsync(null);
          param.Cancelled = Viewmodel.HasUnsavedChanges;
          return;
        case ConfirmationResult.Cancel:
          param.Cancelled = true;
          return;
      };
    }
  }
}