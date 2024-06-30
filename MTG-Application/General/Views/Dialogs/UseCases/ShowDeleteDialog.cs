using Microsoft.UI.Xaml;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Views.Dialogs.Controls;
using System.Threading.Tasks;

namespace MTGApplication.General.Views.Dialogs.UseCases;

public class ShowDeleteDialog(XamlRoot root) : ShowDialogUseCase<ConfirmationResult>(root)
{
  protected override async Task<ConfirmationResult> ShowDialog(string title, string message)
    => await DialogService.ShowAsync(Root, new TwoButtonConfirmationDialog(title, message)
    {
      PrimaryButtonText = "Delete",
    });
}