using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.UI.Controls;
using MTGApplication.Interfaces;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Automation.Peers;

namespace MTGApplication.Views
{
  public static class Dialogs
  {
    public static ContentDialog CurrentDialogs { get; set; } = null;

    /// <summary>
    /// Basic class implementation of the <see cref="IDialogWrapper"/> interface
    /// </summary>
    public class ContentDialogWrapper : IDialogWrapper
    {
      public ContentDialog Dialog { get; set; }

      public ContentDialogWrapper(ContentDialog dialog)
      {
        Dialog = dialog;

        // Add event to close the dialog when user clicks outside of the dialog.
        Dialog.Loaded += (sender, e) =>
        {
          var root = VisualTreeHelper.GetParent(Dialog);
          var smokeLayer = FindByName(root, "SmokeLayerBackground") as FrameworkElement;
          var pressed = false;

          smokeLayer.PointerPressed += (sender, e) =>
          {
            pressed = true;
          };
          smokeLayer.PointerReleased += (sender, e) =>
          {
            if (pressed == true) { Dialog.Hide(); }
            pressed = false;
          };
        };
      }

      public async Task<ContentDialogResult> ShowAsync()
      {
        // Only one dialog can be open
        if(CurrentDialogs != null) { return ContentDialogResult.None; }
        CurrentDialogs = Dialog;
        if(CurrentDialogs != Dialog) { return ContentDialogResult.None; }
        var result = await Dialog.ShowAsync();
        CurrentDialogs = null;
        return result;
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

    public class CheckBoxDialog : DialogBase<(bool? answer, bool? isChecked)>, IInputDialog<bool?>
    {
      protected CheckBox checkBox;

      public string Message = string.Empty;
      public string InputText { get; set; } = string.Empty;
      public bool InputDefaultValue { get; set; } = false;

      public CheckBoxDialog(string title) : base(title) { }

      protected virtual CheckBox CreateCheckBox()
      {
        return new CheckBox
        {
          IsChecked = InputDefaultValue,
          Content = InputText,
        };
      }

      protected override IDialogWrapper CreateDialog(FrameworkElement root)
      {
        var dialog = base.CreateDialog(root);
        checkBox = CreateCheckBox();
        dialog.Dialog.Content = new StackPanel() 
        { 
          Orientation = Orientation.Vertical,
          Children =
          {
            new TextBlock() { Text = Message },
            checkBox
          }
        };
        return dialog;
      }

      protected override (bool? answer, bool? isChecked) ProcessResult(ContentDialogResult result)
      {
        return result switch
        {
          ContentDialogResult.Primary => (answer: true, isChecked: GetInputValue()),
          _ => (answer: null, isChecked: GetInputValue()),
        };
      }

      public virtual bool? GetInputValue() => checkBox.IsChecked;
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
          PlaceholderText = InputPlaceholderText,
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
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
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

        dialog.Dialog.Loaded += (sender, e) =>
        {
          var root = VisualTreeHelper.GetParent(dialog.Dialog);
          var primaryButton = FindByName(root, "PrimaryButton") as Button;

          // Add event to click the primary button when selected item has been double tapped.
          (dialog.Dialog.Content as GridView).DoubleTapped += (sender, e) =>
          {
            var PrimaryFeap = FrameworkElementAutomationPeer.FromElement(primaryButton) as ButtonAutomationPeer;
            if (PrimaryFeap != null) { PrimaryFeap?.Invoke(); } // Click the primary button
            else
            {
              // If primary button is not available, press close button
              var closeButton = FindByName(root, "CloseButton") as Button;
              var closeFeap = FrameworkElementAutomationPeer.FromElement(closeButton) as ButtonAutomationPeer;
              closeFeap?.Invoke();
            }
          };
        };

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

    /// <summary>
    /// Returns <see cref="DependencyObject"/> from the <paramref name="root"/> that has the given <paramref name="name"/>
    /// Searches every child element and child of the children elements
    /// </summary>
    private static DependencyObject FindByName(DependencyObject root, string name)
    {
      var childCount = VisualTreeHelper.GetChildrenCount(root);
      for (int i = 0; i < childCount; i++)
      {
        var child = VisualTreeHelper.GetChild(root, i);
        if (child is not null)
        {
          if (child.GetValue(FrameworkElement.NameProperty) is string value && value == name)
          {
            return child;
          }
          else
          {
            var recursiveResult = FindByName(child, name);
            if (recursiveResult is not null && recursiveResult.GetValue(FrameworkElement.NameProperty) is string recValue && recValue == name)
            {
              return recursiveResult;
            }
          }
        }
      }
      return null;
    }
  }
}
