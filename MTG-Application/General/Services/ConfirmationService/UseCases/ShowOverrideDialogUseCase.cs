using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Services.ConfirmationService;

public class ShowOverrideDialogUseCase : ShowDialogUseCase<ConfirmationResult>
{
  public ShowOverrideDialogUseCase(DialogWrapper dialogWrapper) : base(dialogWrapper) { }

  protected override async Task<ConfirmationResult> ShowDialog(string title, string message) => (await new ConfirmationDialog(title)
  {
    Message = message,
    PrimaryButtonText = "Override",
    SecondaryButtonText = string.Empty
  }.ShowAsync(DialogWrapper)).ToConfirmationResult();
}
