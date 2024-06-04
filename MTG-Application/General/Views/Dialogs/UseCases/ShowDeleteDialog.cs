using MTGApplication.General.Services.ConfirmationService;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Views.Dialogs;

public class ShowDeleteDialog(DialogWrapper dialogWrapper) : ShowDialogUseCase<ConfirmationResult>(dialogWrapper)
{
  protected override async Task<ConfirmationResult> ShowDialog(string title, string message) => (await new ConfirmationDialog(title)
  {
    Message = message,
    PrimaryButtonText = "Delete",
    SecondaryButtonText = string.Empty
  }.ShowAsync(DialogWrapper)).ToConfirmationResult();
}