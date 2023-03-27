using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Services;

public static class DialogService
{
  /// <summary>
  /// Class that can be used to call dialog's showAsync method.
  /// <see cref="TestDialogWrapper"/> can be used to unit test dialogs without calling UI thread.
  /// </summary>
  public class DialogWrapper
  {
    public virtual async Task<ContentDialogResult> ShowAsync(Dialog dialog)
    {
      var contentDialog = dialog.GetDialog();

      // Only one dialog can be open
      if (CurrentDialog != null)
      { return ContentDialogResult.None; }
      CurrentDialog = contentDialog;
      var result = await contentDialog.ShowAsync();
      CurrentDialog = null;
      return result;
    }
  }

  /// <summary>
  /// Base class for dialogs
  /// </summary>
  public abstract class Dialog
  {
    public string Title { get; init; }
    public string PrimaryButtonText { get; init; } = "Yes";
    public string SecondaryButtonText { get; init; } = "No";
    public string CloseButtonText { get; init; } = "Cancel";

    public Dialog(string title) => Title = title;

    /// <summary>
    /// Creates the <see cref="ContentDialog"/>
    /// </summary>
    /// <returns></returns>
    public virtual ContentDialog GetDialog()
    {
      var dialog = new ContentDialog()
      {
        Title = Title,
        XamlRoot = DialogRoot.XamlRoot,
        RequestedTheme = DialogRoot.ActualTheme,
        DefaultButton = ContentDialogButton.Primary,
        PrimaryButtonText = PrimaryButtonText,
        SecondaryButtonText = SecondaryButtonText,
        CloseButtonText = CloseButtonText,
      };

      // Add event to close the dialog when user clicks outside of the dialog.
      dialog.Loaded += (sender, e) =>
      {
        var root = VisualTreeHelper.GetParent(dialog);
        var smokeLayer = FindByName(root, "SmokeLayerBackground") as FrameworkElement;
        var pressed = false;

        smokeLayer.PointerPressed += (sender, e) =>
        {
          pressed = true;
        };
        smokeLayer.PointerReleased += (sender, e) =>
        {
          if (pressed == true)
          { dialog.Hide(); }
          pressed = false;
        };
      };

      return dialog;
    }

    /// <summary>
    /// Shows the dialog to the user using <see cref="CurrentDialogWrapper"/>
    /// </summary>
    public async Task<ContentDialogResult> ShowAsync() => await CurrentDialogWrapper.ShowAsync(this);
  }

  /// <summary>
  /// Base class for dialogs that returns a value.
  /// Returned value is <typeparamref name="T"/>
  /// </summary>
  public abstract class Dialog<T> : Dialog
  {
    public Dialog(string title) : base(title) { }

    /// <summary>
    /// Returns a value depending on the <paramref name="result"/>
    /// </summary>
    public abstract T ProcessResult(ContentDialogResult result);

    /// <summary>
    /// Shows the dialog to the user and returns <typeparamref name="T"/>.
    /// The returned value depends on the user's answer.
    /// </summary>
    public new async Task<T> ShowAsync() => ProcessResult(await base.ShowAsync());
  }

  public static FrameworkElement DialogRoot { get; set; } // MainWindow's root that will be used to show the dialogs
  public static ContentDialog CurrentDialog { get; set; } = null; // Dialog that is currently open
  public static DialogWrapper CurrentDialogWrapper { get; set; } = new(); // Wrapper that will be used to show the dialogs

  /// <summary>
  /// Dialog that asks confirmation from the used.
  /// </summary>
  public class ConfirmationDialog : Dialog<bool?>
  {
    public string Message { get; set; }

    public ConfirmationDialog(string title = "") : base(title) { }

    public override ContentDialog GetDialog()
    {
      var dialog = base.GetDialog();
      dialog.Content = Message;
      return dialog;
    }

    public override bool? ProcessResult(ContentDialogResult result)
    {
      return result switch
      {
        ContentDialogResult.Primary => true,
        ContentDialogResult.Secondary => false,
        _ => null
      };
    }
  }

  /// <summary>
  /// Dialog that shows a message to the used.
  /// Dialog has only close button
  /// </summary>
  public class MessageDialog : Dialog<bool?>
  {
    public string Message { get; set; }

    public MessageDialog(string title = "") : base(title) { }

    public override ContentDialog GetDialog()
    {
      var dialog = base.GetDialog();
      dialog.Content = Message;
      dialog.CloseButtonText = "Close";
      dialog.PrimaryButtonText = string.Empty;
      dialog.SecondaryButtonText = string.Empty;
      return dialog;
    }

    public override bool? ProcessResult(ContentDialogResult result) => true;
  }

  /// <summary>
  /// Dialog with a checkbox
  /// </summary>
  public class CheckBoxDialog : Dialog<(bool? answer, bool? isChecked)>
  {
    protected CheckBox checkBox;
    protected TextBlock textBlock;

    public string Message { get; set; }
    public string InputText { get; set; }
    public bool? IsChecked { get; set; }
    public bool InputDefaultValue { get; set; }

    public CheckBoxDialog(string title = "") : base(title) { }

    public override ContentDialog GetDialog()
    {
      checkBox = new()
      {
        IsChecked = InputDefaultValue,
        Content = InputText,
      };
      textBlock = new()
      {
        Text = Message
      };

      var dialog = base.GetDialog();
      dialog.Content = new StackPanel()
      {
        Orientation = Orientation.Vertical,
        Children =
        {
          textBlock,
          checkBox
        }
      };

      checkBox.Checked += (s, e) => { IsChecked = checkBox.IsChecked; };
      checkBox.Unchecked += (s, e) => { IsChecked = checkBox.IsChecked; };
      return dialog;
    }

    public override (bool? answer, bool? isChecked) ProcessResult(ContentDialogResult result)
    {
      return result switch
      {
        ContentDialogResult.Primary => (answer: true, isChecked: IsChecked),
        _ => (answer: null, isChecked: IsChecked),
      };
    }
  }

  /// <summary>
  /// Dialog with a textbox input
  /// </summary>
  public class TextBoxDialog : Dialog<string>
  {
    protected TextBox textBox;

    public string TextInputText { get; set; } = "";
    public string InputHeaderText { get; set; } = "";
    public string InputPlaceholderText { get; set; } = "";
    public char[] InvalidInputCharacters { get; init; } = Array.Empty<char>();
    public bool IsSpellCheckEnabled { get; set; }

    public TextBoxDialog(string title = "") : base(title) { }

    public override ContentDialog GetDialog()
    {
      textBox = new()
      {
        Header = InputHeaderText,
        IsSpellCheckEnabled = IsSpellCheckEnabled,
        PlaceholderText = InputPlaceholderText,
        Text = TextInputText,
        SelectionStart = TextInputText.Length,
      };
      var dialog = base.GetDialog();
      dialog.Content = textBox;

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

      TextInputText = textBox.Text;
      textBox.TextChanged += (s, e) => { TextInputText = textBox.Text; };
      return dialog;
    }

    public override string ProcessResult(ContentDialogResult result)
    {
      return result switch
      {
        ContentDialogResult.Primary => TextInputText,
        ContentDialogResult.Secondary => string.Empty,
        _ => null,
      };
    }
  }

  /// <summary>
  /// Dialog with a text area input
  /// </summary>
  public class TextAreaDialog : Dialog<string>
  {
    protected TextBox textBox;

    public string TextInputText { get; set; } = "";
    public string InputHeaderText { get; set; } = "";
    public string InputPlaceholderText { get; set; } = "";
    public char[] InvalidInputCharacters { get; init; } = Array.Empty<char>();
    public bool IsSpellCheckEnabled { get; set; }

    public TextAreaDialog(string title = "") : base(title) { }

    public override ContentDialog GetDialog()
    {
      textBox = new()
      {
        Header = InputHeaderText,
        AcceptsReturn = true,
        IsSpellCheckEnabled = IsSpellCheckEnabled,
        PlaceholderText = InputPlaceholderText,
        Text = TextInputText,
        SelectionStart = TextInputText.Length,
        Height = 500,
        Width = 800,
      };
      var dialog = base.GetDialog();
      dialog.Content = textBox;

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

      TextInputText = textBox.Text;
      textBox.TextChanged += (s, e) => { TextInputText = textBox.Text; };
      return dialog;
    }

    public override string ProcessResult(ContentDialogResult result)
    {
      return result switch
      {
        ContentDialogResult.Primary => TextInputText,
        ContentDialogResult.Secondary => string.Empty,
        _ => null,
      };
    }
  }

  /// <summary>
  /// Dialog with a combobox input
  /// </summary>
  public class ComboBoxDialog : Dialog<string>
  {
    protected ComboBox comboBox;

    public string Selection { get; set; }
    public string InputHeader { get; set; }
    public string[] Items { get; set; }

    public ComboBoxDialog(string title = "") : base(title) { }

    public override ContentDialog GetDialog()
    {
      comboBox = new ComboBox()
      {
        ItemsSource = Items,
        Header = InputHeader,
      };
      var dialog = base.GetDialog();
      dialog.Content = comboBox;

      comboBox.SelectionChanged += (s, e) => { Selection = (string)comboBox.SelectedValue; };
      return dialog;
    }

    public override string ProcessResult(ContentDialogResult result)
    {
      return result switch
      {
        ContentDialogResult.Primary => Selection,
        ContentDialogResult.Secondary => string.Empty,
        _ => null,
      };
    }
  }

  /// <summary>
  /// Dialog with a gridview input
  /// </summary>
  public class GridViewDialog : Dialog<object>
  {
    protected GridView gridView;

    public object Selection { get; set; }
    public object[] Items { get; set; }
    public object GridStyle { get; }
    public object GridItemTemplate { get; }

    public GridViewDialog(string title = "", string itemTemplate = "", string gridStyle = "") : base(title)
    {
      GridStyle = gridStyle;
      GridItemTemplate = itemTemplate;
    }

    public override ContentDialog GetDialog()
    {
      Application.Current.Resources.TryGetValue(GridItemTemplate, out var template);
      Application.Current.Resources.TryGetValue(GridStyle, out var style);

      gridView = new AdaptiveGridView()
      {
        DesiredWidth = 250,
        Style = (Style)style,
        ItemTemplate = (DataTemplate)template,
        ItemsSource = Items,
      };

      var dialog = base.GetDialog();
      dialog.Content = gridView;

      gridView.SelectionChanged += (s, e) => { Selection = gridView.SelectedItem; };

      dialog.Loaded += (sender, e) =>
      {
        var root = VisualTreeHelper.GetParent(dialog);
        var primaryButton = FindByName(root, "PrimaryButton") as Button;

        // Add event to click the primary button when selected item has been double tapped.
        (dialog.Content as GridView).DoubleTapped += (sender, e) =>
        {
          var PrimaryFeap = FrameworkElementAutomationPeer.FromElement(primaryButton) as ButtonAutomationPeer;
          if (PrimaryFeap != null)
          { PrimaryFeap?.Invoke(); } // Click the primary button
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

    public override object ProcessResult(ContentDialogResult result)
    {
      return result switch
      {
        ContentDialogResult.Primary => Selection,
        _ => null
      };
    }
  }

  /// <summary>
  /// Returns <see cref="DependencyObject"/> from the <paramref name="root"/> that has the given <paramref name="name"/>
  /// Searches every child element and child of the children elements
  /// </summary>
  private static DependencyObject FindByName(DependencyObject root, string name)
  {
    var childCount = VisualTreeHelper.GetChildrenCount(root);
    for (var i = 0; i < childCount; i++)
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
