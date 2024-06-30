using Microsoft.UI.Xaml;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Views.Dialogs.Controls;
using System.Threading.Tasks;

namespace MTGApplication.General.Views.Dialogs.UseCases;

public class ShowOpenDialog(XamlRoot root) : ShowDialogUseCase<string, string[]>(root)
{
  protected override async Task<string> ShowDialog(string title, string message, string[] data)
   => await DialogService.ShowAsync(Root, new ComboBoxDialog(title, data)
   {
     InputHeader = message,
     PrimaryButtonText = "Open",
     SecondaryButtonText = string.Empty,
   });
}