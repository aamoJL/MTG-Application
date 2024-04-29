using MTGApplication.General.Services.ConfirmationService;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Views.Dialogs;

public class ShowUnsavedChangesDialog : ShowDialogUseCase<ConfirmationResult>
{
  public ShowUnsavedChangesDialog(DialogWrapper dialogWrapper) : base(dialogWrapper) { }

  protected override async Task<ConfirmationResult> ShowDialog(string title, string message) => (await new ConfirmationDialog(title)
  {
    Message = message,
    PrimaryButtonText = "Save"
  }.ShowAsync(DialogWrapper)).ToConfirmationResult();
}
