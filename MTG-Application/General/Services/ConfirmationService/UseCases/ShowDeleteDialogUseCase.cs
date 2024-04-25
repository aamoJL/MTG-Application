using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Services.ConfirmationService;

public class ShowDeleteDialogUseCase : ShowDialogUseCase<ConfirmationResult>
{
  public ShowDeleteDialogUseCase(DialogWrapper dialogWrapper) : base(dialogWrapper) { }

  protected override async Task<ConfirmationResult> ShowDialog(string title, string message) => (await new ConfirmationDialog(title)
  {
    Message = message,
    PrimaryButtonText = "Delete",
    SecondaryButtonText = string.Empty
  }.ShowAsync(DialogWrapper)).ToConfirmationResult();
}
