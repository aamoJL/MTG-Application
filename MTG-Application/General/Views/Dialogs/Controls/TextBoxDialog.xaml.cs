using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.ComponentModel;

namespace MTGApplication.General.Views.Dialogs.Controls;

public sealed partial class TextBoxDialog : StringDialog, INotifyPropertyChanged
{
  public TextBoxDialog(string title) : base(title)
  {
    InitializeComponent();
    SecondaryButtonText = string.Empty;

    Loaded += TextBoxDialog_Loaded;
  }

  private void TextBoxDialog_Loaded(object sender, RoutedEventArgs e)
  {
    Loaded -= TextBoxDialog_Loaded;

    ErrorMessage = ErrorCheck(InputText);
  }

  public string InputText { get; set; } = string.Empty;
  public string InputHeader { get; set; } = string.Empty;
  public string InputPlaceholderText { get; set; } = string.Empty;
  public char[] InvalidInputCharacters { get; set; } = [];
  public bool IsSpellCheckEnabled { get; set; } = false;

  private string? ErrorMessage
  {
    get;
    set
    {
      if (field == value) return;

      field = value;
      IsPrimaryButtonEnabled = field == null;
      PropertyChanged?.Invoke(this, new(nameof(ErrorMessage)));
      PropertyChanged?.Invoke(this, new(nameof(ErrorVisibility)));
    }
  } = null;
  private Visibility ErrorVisibility => !string.IsNullOrEmpty(ErrorMessage) ? Visibility.Visible : Visibility.Collapsed;

  public Func<string, string?>? InputErrorValidation { get; set; } = null;

  public event PropertyChangedEventHandler? PropertyChanged;

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

    ErrorMessage = ErrorCheck(text);
  }

  private string? ErrorCheck(string text)
  {
    if (InputErrorValidation != null) return InputErrorValidation(text);
    else return null;
  }
}
