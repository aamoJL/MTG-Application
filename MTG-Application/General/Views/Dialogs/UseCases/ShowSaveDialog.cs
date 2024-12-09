using Microsoft.UI.Xaml;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Views.Dialogs.Controls;
using System.IO;
using System.Threading.Tasks;

namespace MTGApplication.General.Views.Dialogs.UseCases;

public class ShowSaveDialog(XamlRoot root) : ShowDialogUseCase<string?, string>(root)
{
  protected override async Task<string?> ShowDialog(string title, string message, string data)
    => await DialogService.ShowAsync(Root, new TextBoxDialog(title)
    {
      InvalidInputCharacters = Path.GetInvalidFileNameChars(),
      InputText = data,
      PrimaryButtonText = "Save",
      InputValidation = input => !string.IsNullOrEmpty(input)
    });
}