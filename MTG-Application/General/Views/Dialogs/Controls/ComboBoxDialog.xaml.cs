using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.General.Views.Dialogs.Controls;

public sealed partial class ComboBoxDialog : StringDialog
{
  public ComboBoxDialog(string title, string[] items) : base(title)
  {
    InitializeComponent();

    Items = items;
  }

  public string[] Items { get; }
  public string InputHeader { get; set; } = string.Empty;
  public string? Selection { get; set; } = null;

  protected override string? ProcessResult(ContentDialogResult result)
  {
    return result switch
    {
      ContentDialogResult.Primary => Selection,
      ContentDialogResult.Secondary => string.Empty,
      _ => null,
    };
  }
}
