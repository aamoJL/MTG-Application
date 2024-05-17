using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Views.Dialogs;

public class ShowExportDialog : ShowDialogUseCase<string, string>
{
  public ShowExportDialog(DialogWrapper dialogWrapper) : base(dialogWrapper) { }

  protected override async Task<string> ShowDialog(string title, string message, string data)
  {
    return await new TextAreaDialog(title)
    {
      TextInputText = data,
      PrimaryButtonText = "Copy to Clipboard",
      SecondaryButtonText = string.Empty
    }.ShowAsync(DialogWrapper);
  }
}
