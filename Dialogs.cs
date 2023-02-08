using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MTGApplication
{
  // TODO: Separate the UI from the methods with interfaces
  public static class Dialogs
  {
    public static async Task MessageDialogAsync(this FrameworkElement element, string title, string message)
    {
      await MessageDialogAsync(element, title, message, "OK");
    }

    public static async Task MessageDialogAsync(this FrameworkElement element, string title, string message, string buttonText)
    {
      var dialog = new ContentDialog
      {
        Title = title,
        Content = message,
        CloseButtonText = buttonText,
        XamlRoot = element.XamlRoot,
        RequestedTheme = element.ActualTheme,
      };

      await dialog.ShowAsync();
    }

    public static async Task<bool?> ConfirmationDialogAsync(
     this FrameworkElement element,
     string title,
     object content,
     string yesButtonText = "Yes",
     string noButtonText = "No",
     string cancelButtonText = "Cancel")
    {
      var dialog = new ContentDialog
      {
        Title = title,
        PrimaryButtonText = yesButtonText,
        SecondaryButtonText = noButtonText,
        CloseButtonText = cancelButtonText,
        XamlRoot = element.XamlRoot,
        RequestedTheme = element.ActualTheme,
        Content = content,
        DefaultButton = ContentDialogButton.Primary
      };
      var result = await dialog.ShowAsync();

      if (result == ContentDialogResult.None)
      {
        return null;
      }

      return (result == ContentDialogResult.Primary);
    }

    public static async Task<string> InputStringDialogAsync(
     this FrameworkElement element,
     string title,
     string defaultText = "",
     string okButtonText = "OK",
     string cancelButtonText = "Cancel",
     char[] invalidCharacters = null)
    {
      var inputTextBox = new TextBox
      {
        AcceptsReturn = false,
        Text = defaultText,
        SelectionStart = defaultText.Length,
      };
      var dialog = new ContentDialog
      {
        Content = inputTextBox,
        Title = title,
        IsSecondaryButtonEnabled = true,
        PrimaryButtonText = okButtonText,
        SecondaryButtonText = cancelButtonText,
        XamlRoot = element.XamlRoot,
        RequestedTheme = element.ActualTheme,
        DefaultButton = ContentDialogButton.Primary
      };

      if(invalidCharacters != null)
      {
        inputTextBox.TextChanging += (TextBox sender, TextBoxTextChangingEventArgs args) =>
        {
          var text = sender.Text;
          foreach (var c in Path.GetInvalidFileNameChars())
          {
            text = text.Replace(c.ToString(), string.Empty);
          }
          var oldSelectionStart = sender.SelectionStart;
          sender.Text = text;
          sender.Select(oldSelectionStart, 0);
        };
      }

      if (await dialog.ShowAsync() == ContentDialogResult.Primary)
      {
        return inputTextBox.Text;
      }
      else
      {
        return string.Empty;
      }
    }

    public static async Task<string> TextAreaInputDialogAsync(
     this FrameworkElement element,
     string title,
     string inputPlaceholder = "",
     string inputHeader = null,
     string defaultText = "",
     string okButtonText = "OK",
     string cancelButtonText = "Cancel")
    {
      var inputTextBox = new TextBox
      {
        PlaceholderText = inputPlaceholder,
        IsSpellCheckEnabled = false,
        Header = inputHeader,
        AcceptsReturn = true,
        Text = defaultText,
        Height = 600,
        Width = 800,
      };
      var dialog = new ContentDialog
      {
        Content = inputTextBox,
        Title = title,
        PrimaryButtonText = okButtonText,
        SecondaryButtonText = cancelButtonText,
        XamlRoot = element.XamlRoot,
        RequestedTheme = element.ActualTheme,
        DefaultButton = ContentDialogButton.Primary
      };

      if (await dialog.ShowAsync() == ContentDialogResult.Primary)
      {
        return inputTextBox.Text;
      }
      else
      {
        return string.Empty;
      }
    }

    public static async Task<string> ComboboxDialogAsync(
     this FrameworkElement element,
     string title,
     string[] items,
     string header = "",
     string okButtonText = "OK",
     string cancelButtonText = "Cancel")
    {
      var inputComboBox = new ComboBox
      {
        ItemsSource = items,
        Header = header
      };
      var dialog = new ContentDialog
      {
        Content = inputComboBox,
        Title = title,
        IsSecondaryButtonEnabled = true,
        PrimaryButtonText = okButtonText,
        SecondaryButtonText = cancelButtonText,
        XamlRoot = element.XamlRoot,
        RequestedTheme = element.ActualTheme,
        DefaultButton = ContentDialogButton.Primary
      };

      if (await dialog.ShowAsync() == ContentDialogResult.Primary && inputComboBox.SelectedValue != null)
      {
        return inputComboBox.SelectedValue.ToString();
      }
      else
      {
        return string.Empty;
      }
    }
  }

  public static class Notifications
  {
    public static event EventHandler<string> OnCopied;
    public static event EventHandler<string> OnError;

    public static void RaiseCopied(string text) => OnCopied?.Invoke(null, text);
    public static void RaiseError(string text) => OnError?.Invoke(null, text);
  }
}
