using MTGApplication.General.Views.Dialogs.Controls;
using System.IO;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Views.Dialogs.UseCases;

public class ShowSaveDialog(DialogWrapper dialogWrapper) : ShowDialogUseCase<string, string>(dialogWrapper)
{
  protected override async Task<string> ShowDialog(string title, string message, string data)
    => await DialogWrapper.ShowAsync(new TextBoxDialog(title)
    {
      InvalidInputCharacters = Path.GetInvalidFileNameChars(),
      InputText = data,
      PrimaryButtonText = "Save",
    });
}