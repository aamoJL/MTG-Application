using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using System.IO;
using CommunityToolkit.WinUI.UI.Controls;
using MTGApplication.Interfaces;

namespace MTGApplication.Views
{
  public static class Dialogs
  {
    /// <summary>
    /// Basic class implementation of the <see cref="IDialogWrapper"/> interface
    /// </summary>
    public class ContentDialogWrapper : IDialogWrapper
    {
      public ContentDialog Dialog { get; set; }

      public ContentDialogWrapper(ContentDialog dialog)
      {
        Dialog = dialog;
      }

      public async Task<ContentDialogResult> ShowAsync()
      {
        return await Dialog.ShowAsync();
      }
    }

    /// <summary>
    /// Generic base class for dialogs
    /// </summary>
    public abstract class DialogBase<T>
    {
      protected string Title { get; }
      public string PrimaryButtonText { get; set; } = "Yes";
      public string SecondaryButtonText { get; set; } = "No";
      public string CloseButtonText { get; set; } = "Cancel";

      public DialogBase(string title)
      {
        Title = title;
      }

      /// <summary>
      /// Shows dialog to the user and returns requested object
      /// </summary>
      public async Task<T> ShowAsync(FrameworkElement root)
      {
        var result = await CreateDialog(root).ShowAsync();
        return ProcessResult(result);
      }

      /// <summary>
      /// Creates the dialog that will be shown to the user
      /// </summary>
      protected virtual IDialogWrapper CreateDialog(FrameworkElement root)
      {
        return new ContentDialogWrapper(new ContentDialog()
        {
          Title = Title,
          RequestedTheme = root.ActualTheme,
          XamlRoot = root.XamlRoot,
          DefaultButton = ContentDialogButton.Primary,
          CloseButtonText = CloseButtonText,
          PrimaryButtonText = PrimaryButtonText,
          SecondaryButtonText = SecondaryButtonText,
        });
      }

      /// <summary>
      /// Returns object that matches the given result for the dialog
      /// </summary>
      protected abstract T ProcessResult(ContentDialogResult result);
    }

    /// <summary>
    /// Dialog that can only be closed
    /// </summary>
    public class MessageDialog : DialogBase<bool?>
    {
      public string Message { get; set; } = string.Empty;

      public MessageDialog(string title) : base(title) { }

      protected override IDialogWrapper CreateDialog(FrameworkElement root)
      {
        var dialog = base.CreateDialog(root);
        dialog.Dialog.Content = Message;
        dialog.Dialog.PrimaryButtonText = string.Empty;
        dialog.Dialog.SecondaryButtonText = string.Empty;
        return dialog;
      }

      protected override bool? ProcessResult(ContentDialogResult result) => true;
    }

    /// <summary>
    /// Dialog without secondary inputs
    /// </summary>
    public class ConfirmationDialog : DialogBase<bool?>
    {
      public string Message = string.Empty;

      public ConfirmationDialog(string title) : base(title) { }

      protected override IDialogWrapper CreateDialog(FrameworkElement root)
      {
        var dialog = base.CreateDialog(root);
        dialog.Dialog.Content = Message;
        return dialog;
      }

      protected override bool? ProcessResult(ContentDialogResult result)
      {
        return result switch
        {
          ContentDialogResult.Primary => true,
          ContentDialogResult.Secondary => false,
          _ => null,
        };
      }
    }

    /// <summary>
    /// Dialog with a textbox input
    /// </summary>
    public class TextBoxDialog : DialogBase<string>, IInputDialog<string>
    {
      protected TextBox textBox;

      public string InputHeaderText { get; set; } = string.Empty;
      public string InputDefaultText { get; set; } = string.Empty;
      public string InputPlaceholderText { get; set; } = string.Empty;
      public char[] InvalidInputCharacters { get; set; } = Array.Empty<char>();
      public bool IsSpellCheckEnabled { get; set; } = false;

      public TextBoxDialog(string title) : base(title)
      {
        PrimaryButtonText = "OK";
      }

      protected virtual TextBox CreateTextBox(bool multiline = false)
      {
        return new TextBox()
        {
          Header = InputHeaderText,
          AcceptsReturn = multiline,
          IsSpellCheckEnabled = IsSpellCheckEnabled,
          Text = InputDefaultText,
          SelectionStart = InputDefaultText.Length,
        };
      }

      protected override IDialogWrapper CreateDialog(FrameworkElement root)
      {
        var dialog = base.CreateDialog(root);

        textBox = CreateTextBox();

        dialog.Dialog.Content = textBox;

        if (InvalidInputCharacters.Length > 0)
        {
          textBox.TextChanging += (sender, args) =>
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

        return dialog;
      }

      protected override string ProcessResult(ContentDialogResult result)
      {
        return result switch
        {
          ContentDialogResult.Primary => GetInputValue(),
          ContentDialogResult.Secondary => string.Empty,
          _ => null,
        };
      }

      public virtual string GetInputValue() => textBox.Text;
    }

    /// <summary>
    /// Dialog with a text area input
    /// </summary>
    public class TextAreaDialog : TextBoxDialog
    {
      public TextAreaDialog(string title) : base(title) { }

      protected override TextBox CreateTextBox(bool multipline)
      {
        var textBox = base.CreateTextBox(true);
        textBox.AcceptsReturn = true;
        textBox.Height = 500;
        textBox.Width = 800;
        return textBox;
      }
    }

    /// <summary>
    /// Dialog with a combobox input
    /// </summary>
    public class ComboBoxDialog : DialogBase<string>, IInputDialog<string>
    {
      protected ComboBox comboBox;

      public string InputHeader { get; set; } = string.Empty;
      public string[] Items { get; set; } = Array.Empty<string>();

      public ComboBoxDialog(string title) : base(title) { }

      protected ComboBox CreateComboBox()
      {
        return new ComboBox()
        {
          ItemsSource = Items,
          Header = InputHeader,
        };
      }

      protected override IDialogWrapper CreateDialog(FrameworkElement root)
      {
        var dialog = base.CreateDialog(root);
        comboBox = CreateComboBox();
        dialog.Dialog.Content = comboBox;
        return dialog;
      }

      protected override string ProcessResult(ContentDialogResult result)
      {
        return result switch
        {
          ContentDialogResult.Primary => GetInputValue(),
          ContentDialogResult.Secondary => string.Empty,
          _ => null,
        };
      }

      public virtual string GetInputValue() => (string)comboBox.SelectedValue;
    }

    /// <summary>
    /// Dialog with a gridview input
    /// </summary>
    public class GridViewDialog : DialogBase<object>, IInputDialog<object>
    {
      protected GridView gridView;

      public string ItemTemplateName { get; set; } = string.Empty;
      public string GridViewStyleName { get; set; } = string.Empty;
      public object[] Items { get; set; } = Array.Empty<object>();

      public GridViewDialog(string title) : base(title) { }

      protected GridView CreateGridView()
      {
        Application.Current.Resources.TryGetValue(ItemTemplateName, out object template);
        Application.Current.Resources.TryGetValue(GridViewStyleName, out object style);

        return new AdaptiveGridView()
        {
          ItemsSource = Items,
          DesiredWidth = 250,
          Style = (Style)style,
          ItemTemplate = (DataTemplate)template
        };
      }

      protected override IDialogWrapper CreateDialog(FrameworkElement root)
      {
        var dialog = base.CreateDialog(root);
        gridView = CreateGridView();
        dialog.Dialog.Content = gridView;
        return dialog;
      }

      protected override object ProcessResult(ContentDialogResult result)
      {
        return result switch
        {
          ContentDialogResult.Primary => GetInputValue(),
          _ => null
        };
      }

      public virtual object GetInputValue() => gridView.SelectedValue;
    }
  }
}
