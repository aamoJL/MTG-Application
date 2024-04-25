using System.Threading.Tasks;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.General.Services.ConfirmationService;

public class ShowOpenDialogUseCase : ShowDialogUseCase<string, string[]>
{
  public ShowOpenDialogUseCase(DialogWrapper dialogWrapper) : base(dialogWrapper) { }

  protected override async Task<string> ShowDialog(string title, string message, string[] data) => await new ComboBoxDialog(title)
  {
    InputHeader = message,
    Items = data,
    PrimaryButtonText = "Open",
    SecondaryButtonText = string.Empty
  }.ShowAsync(DialogWrapper);
}