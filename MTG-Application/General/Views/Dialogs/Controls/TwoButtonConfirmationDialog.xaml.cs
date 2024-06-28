using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.General.Views.Dialogs.Controls;
public sealed partial class TwoButtonConfirmationDialog : ConfirmationDialog
{
  public TwoButtonConfirmationDialog(string title, string message) : base(title)
  {
    InitializeComponent();

    Message = message;

    SecondaryButtonText = string.Empty;
  }

  public string Message { get; }

  protected override ConfirmationResult ProcessResult(ContentDialogResult result)
  {
    return result switch
    {
      ContentDialogResult.Primary => ConfirmationResult.Yes,
      _ => ConfirmationResult.Cancel,
    };
  }
}
