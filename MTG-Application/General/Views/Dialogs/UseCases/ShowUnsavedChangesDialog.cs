using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Views.Dialogs.Controls;
using System.Threading.Tasks;

namespace MTGApplication.General.Views.Dialogs.UseCases;

public class ShowUnsavedChangesDialog(DialogService.DialogWrapper dialogWrapper) : ShowDialogUseCase<ConfirmationResult>(dialogWrapper)
{
  protected override async Task<ConfirmationResult> ShowDialog(string title, string message)
    => await DialogWrapper.ShowAsync(force: true, dialog: new ThreeButtonConfirmationDialog(title, message)
    {
      PrimaryButtonText = "Save"
    });
}
