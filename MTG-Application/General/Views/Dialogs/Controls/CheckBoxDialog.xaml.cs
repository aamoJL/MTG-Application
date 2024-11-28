using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.General.Views.Dialogs.Controls;
public sealed partial class CheckBoxDialog : ConfirmationDialogWithBool
{
  public CheckBoxDialog(string title, string message) : base(title)
  {
    InitializeComponent();

    Message = message;

    SecondaryButtonText = string.Empty;
  }

  public string Message { get; } = string.Empty;
  public string InputText { get; set; } = string.Empty;
  public bool IsChecked { get; set; }

  protected override (ConfirmationResult, bool) ProcessResult(ContentDialogResult result)
  {
    var confirmationResult = result switch
    {
      ContentDialogResult.Primary => ConfirmationResult.Yes,
      _ => ConfirmationResult.Cancel,
    };

    return (confirmationResult, IsChecked);
  }
}
