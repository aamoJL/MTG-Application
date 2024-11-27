using Microsoft.UI.Xaml.Controls;
using System;

namespace MTGApplication.General.Views.Dialogs.Controls;
public sealed partial class TextBoxDialog : StringDialog
{
  public TextBoxDialog(string title) : base(title)
  {
    InitializeComponent();
    SecondaryButtonText = string.Empty;
  }

  public string InputText { get; set; } = string.Empty;
  public string InputHeader { get; set; } = string.Empty;
  public string InputPlaceholderText { get; set; } = string.Empty;
  public char[] InvalidInputCharacters { get; set; } = [];
  public bool IsSpellCheckEnabled { get; set; } = false;

  public Func<string, bool>? InputValidation
  {
    get;
    set
    {
      field = value;
      IsPrimaryButtonEnabled = InputValidation?.Invoke(InputText) ?? true;
    }
  }

  protected override string? ProcessResult(ContentDialogResult result)
    => result switch
    {
      ContentDialogResult.Primary => InputText,
      _ => null,
    };

  private void TextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
  {
    if (InvalidInputCharacters.Length == 0)
      return;

    var text = sender.Text;
    var oldSelectionStart = sender.SelectionStart;

    foreach (var c in System.IO.Path.GetInvalidFileNameChars())
      text = text.Replace(c.ToString(), string.Empty);

    sender.Text = text;
    sender.Select(oldSelectionStart, 0);

    IsPrimaryButtonEnabled = InputValidation?.Invoke(text) ?? true;
  }
}
