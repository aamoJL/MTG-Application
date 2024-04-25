using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Services.ConfirmationService;

public class ShowUnsavedChangesDialogUseCase : ShowDialogUseCase<ConfirmationResult>
{
  public ShowUnsavedChangesDialogUseCase(DialogWrapper dialogWrapper) : base(dialogWrapper) { }

  protected override async Task<ConfirmationResult> ShowDialog(string title, string message) => (await new ConfirmationDialog(title)
  {
    Message = message,
    PrimaryButtonText = "Save"
  }.ShowAsync(DialogWrapper)).ToConfirmationResult();
}
