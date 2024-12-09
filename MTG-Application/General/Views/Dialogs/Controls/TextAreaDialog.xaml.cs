using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.General.Views.Dialogs.Controls;
public sealed partial class TextAreaDialog : StringDialog
{
  public TextAreaDialog(string title) : base(title)
  {
    InitializeComponent();

    SecondaryButtonText = string.Empty;

    Loaded += TextAreaDialog_Loaded;
  }

  public string InputText { get; set; } = string.Empty;
  public string InputHeader { get; set; } = string.Empty;
  public string InputPlaceholderText { get; set; } = string.Empty;
  public char[] InvalidInputCharacters { get; set; } = [];
  public bool IsSpellCheckEnabled { get; set; } = false;

  protected override string? ProcessResult(ContentDialogResult result)
    => result switch
    {
      ContentDialogResult.Primary => InputText,
      _ => null,
    };

  private void TextAreaDialog_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
  {
    Loaded -= TextAreaDialog_Loaded;

    if (Content is TextBox box)
      box.SelectionStart = InputText.Length;
  }

  private void TextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
  {
    if (InvalidInputCharacters.Length == 0) return;

    var text = sender.Text;
    var oldSelectionStart = sender.SelectionStart;

    foreach (var c in System.IO.Path.GetInvalidFileNameChars())
      text = text.Replace(c.ToString(), string.Empty);

    sender.Text = text;
    sender.Select(oldSelectionStart, 0);
  }
}
