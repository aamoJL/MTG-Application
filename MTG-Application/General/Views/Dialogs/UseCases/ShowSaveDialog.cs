using System.IO;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Views.Dialogs.UseCases;

public class ShowSaveDialog : ShowDialogUseCase<string, string>
{
  public ShowSaveDialog(DialogWrapper dialogWrapper) : base(dialogWrapper) { }

  protected override async Task<string> ShowDialog(string title, string message, string data) => await new TextBoxDialog(title)
  {
    InvalidInputCharacters = Path.GetInvalidFileNameChars(),
    TextInputText = data,
    PrimaryButtonText = "Save",
    SecondaryButtonText = string.Empty
  }.ShowAsync(DialogWrapper);
}