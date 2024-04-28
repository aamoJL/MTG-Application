using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Services.ConfirmationService;

public class ShowOpenDialog : ShowDialogUseCase<string, string[]>
{
  public ShowOpenDialog(DialogWrapper dialogWrapper) : base(dialogWrapper) { }

  protected override async Task<string> ShowDialog(string title, string message, string[] data) => await new ComboBoxDialog(title)
  {
    InputHeader = message,
    Items = data,
    PrimaryButtonText = "Open",
    SecondaryButtonText = string.Empty
  }.ShowAsync(DialogWrapper);
}