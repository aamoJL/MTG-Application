using MTGApplication.General.Views.Dialogs.Controls;
using System.Threading.Tasks;

namespace MTGApplication.General.Views.Dialogs.UseCases;

public class ShowOpenDialog(Services.ConfirmationService.DialogService.DialogWrapper dialogWrapper) : ShowDialogUseCase<string, string[]>(dialogWrapper)
{
  protected override async Task<string> ShowDialog(string title, string message, string[] data)
   => await DialogWrapper.ShowAsync(new ComboBoxDialog(title, data)
   {
     InputHeader = message,
     PrimaryButtonText = "Open",
     SecondaryButtonText = string.Empty,
   });
}