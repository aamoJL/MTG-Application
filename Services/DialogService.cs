using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using MTGApplication.Interfaces;
using System.IO;

namespace MTGApplication.Services
{
  public static class Dialogs
  {
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

      return result == ContentDialogResult.Primary;
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

      if (invalidCharacters != null)
      {
        inputTextBox.TextChanging += (sender, args) =>
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
        return null;
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
        Height = 500,
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
        return null;
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

  public class DialogService
  {
    public class MessageDialog : IDialog<object>
    {
      public string Title { get; set; } = "Title";
      public string Message { get; set; } = "Message text";
      public string ButtonText { get; set; } = "OK";

      public async virtual Task<object> Show()
      {
        await App.MainRoot.MessageDialogAsync(Title, Message, ButtonText);
        return null;
      }
    }

    public class ComboboxDialog : IDialog<string>
    {
      public string Title { get; set; } = "Title";
      public string[] Items { get; set; } = Array.Empty<string>();
      public string Header { get; set; } = "Header";
      public string OkButtonText { get; set; } = "OK";
      public string CancelButtonText { get; set; } = "Cancel";

      public async virtual Task<string> Show()
      {
        return await App.MainRoot.ComboboxDialogAsync(Title, Items, Header, OkButtonText, CancelButtonText);
      }
    }

    public class InputStringDialog : IDialog<string>
    {
      public string Title { get; set; } = "Title";
      public string DefaultText { get; set; } = string.Empty;
      public string OkButtonText { get; set; } = "OK";
      public string CancelButtonText { get; set; } = "Cancel";
      public char[] InvalidCharacters { get; set; } = Array.Empty<char>();

      public async virtual Task<string> Show()
      {
        return await App.MainRoot.InputStringDialogAsync(Title, DefaultText, OkButtonText, CancelButtonText, InvalidCharacters);
      }
    }

    public class ConfirmationDialog : IDialog<bool?>
    {
      public string Title { get; set; } = "Title";
      public object Content { get; set; } = null;
      public string YesButtonText { get; set; } = "Yes";
      public string NoButtonText { get; set; } = "No";
      public string CancelButtonText { get; set; } = "Cancel";

      public async virtual Task<bool?> Show()
      {
        return await App.MainRoot.ConfirmationDialogAsync(Title, Content, YesButtonText, NoButtonText, CancelButtonText);
      }
    }

    public class InputTextAreaDialog : IDialog<string>
    {
      public string Title { get; set; } = "Title";
      public string InputPlaceholder { get; set; } = string.Empty;
      public string InputHeader { get; set; } = string.Empty;
      public string DefaultText { get; set; } = string.Empty;
      public string OkButtonText { get; set; } = "OK";
      public string CancelButtonText { get; set; } = "Cancel";

      public async virtual Task<string> Show()
      {
        return await App.MainRoot.TextAreaInputDialogAsync(Title, InputPlaceholder, InputHeader, DefaultText, OkButtonText, CancelButtonText);
      }
    }
  }
}
