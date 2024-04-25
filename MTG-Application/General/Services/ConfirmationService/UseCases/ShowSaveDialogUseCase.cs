using System.IO;
using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Services.ConfirmationService;

public class ShowSaveDialogUseCase : ShowDialogUseCase<string, string>
{
  public ShowSaveDialogUseCase(DialogWrapper dialogWrapper) : base(dialogWrapper) { }

  protected override async Task<string> ShowDialog(string title, string message, string data) => (await new TextBoxDialog(title)
  {
    InvalidInputCharacters = Path.GetInvalidFileNameChars(),
    TextInputText = data,
    PrimaryButtonText = "Save",
    SecondaryButtonText = string.Empty
  }.ShowAsync(DialogWrapper));
}