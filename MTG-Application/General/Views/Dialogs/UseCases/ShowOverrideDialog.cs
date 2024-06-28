using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Views.Dialogs.Controls;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Views.Dialogs.UseCases;

public class ShowOverrideDialog(DialogService.DialogWrapper dialogWrapper) : ShowDialogUseCase<ConfirmationResult>(dialogWrapper)
{
  protected override async Task<ConfirmationResult> ShowDialog(string title, string message)
    => await DialogWrapper.ShowAsync(new TwoButtonConfirmationDialog(title, message)
    {
      PrimaryButtonText = "Override"
    });
}
