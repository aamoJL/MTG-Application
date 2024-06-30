using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Services.ConfirmationService;

namespace MTGApplication.General.Views.Dialogs.Controls;
public sealed partial class ThreeButtonConfirmationDialog : ConfirmationDialog
{
  public ThreeButtonConfirmationDialog(string title, string message) : base(title)
  {
    InitializeComponent();

    Message = message;
  }

  public string Message { get; }

  protected override ConfirmationResult ProcessResult(ContentDialogResult result)
  {
    return result switch
    {
      ContentDialogResult.Primary => ConfirmationResult.Yes,
      ContentDialogResult.Secondary => ConfirmationResult.No,
      _ => ConfirmationResult.Cancel,
    };
  }
}
