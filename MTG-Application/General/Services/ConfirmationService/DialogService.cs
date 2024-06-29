using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MTGApplication.General.Views.Dialogs.Controls;
using MTGApplication.General.Views.Extensions;
using System;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.ConfirmationService;

/// <summary>
/// Service to show dialogs on windows.
/// </summary>
public static partial class DialogService
{
  /// <summary>
  /// Class that can be used to call dialog's showAsync method.
  /// <see cref="TestDialogWrapper"/> can be used to unit test dialogs without calling UI thread.
  /// </summary>
  public class DialogWrapper(XamlRoot xamlRoot)
  {
    public ContentDialog CurrentDialog { get; private set; }
    public XamlRoot XamlRoot { get; set; } = xamlRoot;

    public async Task<T> ShowAsync<T>(CustomContentDialog<T> dialog, bool force = false)
    {
      dialog.XamlRoot = XamlRoot;
      var contentDialog = dialog;

      // Only one dialog can be open on a window
      if (force)
        CurrentDialog?.Hide();
      else if (CurrentDialog != null)
        return default;

      CurrentDialog = contentDialog;
      var result = await contentDialog.ShowAsync();

      if (CurrentDialog == contentDialog)
        CurrentDialog = null;

      return result;
    }

    [Obsolete("Use ContentDialogs")]
    public virtual async Task<ContentDialogResult> ShowAsync(Dialog dialog, bool force = false)
    {
      var contentDialog = dialog.GetDialog(XamlRoot);

      // Only one dialog can be open on a window
      if (force)
        CurrentDialog?.Hide();
      else if (CurrentDialog != null)
        return ContentDialogResult.None;

      CurrentDialog = contentDialog;
      var result = await contentDialog.ShowAsync();
      CurrentDialog = null;
      return result;
    }
  }

  /// <summary>
  /// Base class for dialogs
  /// </summary>
  public abstract class Dialog(string title)
  {
    public string Title { get; init; } = title;
    public string PrimaryButtonText { get; init; } = "Yes";
    public string SecondaryButtonText { get; init; } = "No";
    public string CloseButtonText { get; init; } = "Cancel";

    /// <summary>
    /// Creates the <see cref="ContentDialog"/>
    /// </summary>
    /// <returns></returns>
    public virtual ContentDialog GetDialog(XamlRoot root)
    {
      var dialog = new ContentDialog()
      {
        Title = Title,
        XamlRoot = root,
        RequestedTheme = AppConfig.LocalSettings.AppTheme,
        DefaultButton = ContentDialogButton.Primary,
        PrimaryButtonText = PrimaryButtonText,
        SecondaryButtonText = SecondaryButtonText,
        CloseButtonText = CloseButtonText,
      };

      // Add event to close the dialog when user clicks outside of the dialog.
      dialog.Loaded += (sender, e) =>
      {
        var root = VisualTreeHelper.GetParent(dialog);
        var smokeLayer = root.FindChildByName("SmokeLayerBackground") as FrameworkElement;
        var pressed = false;

        smokeLayer.PointerPressed += (sender, e) =>
        {
          pressed = true;
        };
        smokeLayer.PointerReleased += (sender, e) =>
        {
          if (pressed == true) { dialog.Hide(); }
          pressed = false;
        };
      };

      return dialog;
    }

    /// <summary>
    /// Shows the dialog to the user using <see cref="Wrapper"/>
    /// </summary>
    public async Task<ContentDialogResult> ShowAsync(DialogWrapper wrapper, bool force = false)
    {
      if (wrapper == null) { return ContentDialogResult.None; }
      return await wrapper.ShowAsync(this, force);
    }
  }

  /// <summary>
  /// Base class for dialogs that returns a value.
  /// Returned value is <typeparamref name="T"/>
  /// </summary>
  public abstract class Dialog<T>(string title) : Dialog(title)
  {
    /// <summary>
    /// Returns a value depending on the <paramref name="result"/>
    /// </summary>
    public abstract T ProcessResult(ContentDialogResult result);

    /// <summary>
    /// Shows the dialog to the user and returns <typeparamref name="T"/>.
    /// The returned value depends on the user's answer.
    /// </summary>
    public new async Task<T> ShowAsync(DialogWrapper wrapper, bool force = false)
      => ProcessResult(await base.ShowAsync(wrapper, force));
  }
}

// TODO: dialogs to views?
// Basic dialog types
public static partial class DialogService
{
  #region Dialog Types

  /// <summary>
  /// Dialog with a textbox input
  /// </summary>
  public class TextBoxDialog(string title = "") : Dialog<string>(title)
  {
    protected TextBox textBox;

    public string TextInputText { get; set; } = "";
    public string InputHeaderText { get; set; } = "";
    public string InputPlaceholderText { get; set; } = "";
    public char[] InvalidInputCharacters { get; init; } = Array.Empty<char>();
    public bool IsSpellCheckEnabled { get; set; }

    public override ContentDialog GetDialog(XamlRoot root)
    {
      textBox = new()
      {
        Header = InputHeaderText,
        IsSpellCheckEnabled = IsSpellCheckEnabled,
        PlaceholderText = InputPlaceholderText,
        Text = TextInputText,
        SelectionStart = TextInputText.Length,
      };
      var dialog = base.GetDialog(root);
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
  public class TextAreaDialog(string title = "") : Dialog<string>(title)
  {
    protected TextBox textBox;

    public string TextInputText { get; set; } = "";
    public string InputHeaderText { get; set; } = "";
    public string InputPlaceholderText { get; set; } = "";
    public char[] InvalidInputCharacters { get; init; } = Array.Empty<char>();
    public bool IsSpellCheckEnabled { get; set; }

    public override ContentDialog GetDialog(XamlRoot root)
    {
      textBox = new()
      {
        Header = InputHeaderText,
        AcceptsReturn = true,
        IsSpellCheckEnabled = IsSpellCheckEnabled,
        PlaceholderText = InputPlaceholderText,
        Text = TextInputText ?? string.Empty,
        SelectionStart = TextInputText?.Length ?? 0,
        Height = 500,
        Width = 800,
      };
      var dialog = base.GetDialog(root);
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
  /// Dialog with a gridview input
  /// </summary>
  public class GridViewDialog<T>(string title = "", string itemTemplate = "", string gridStyle = "") : Dialog<object>(title)
  {
    protected GridView gridView;

    public T Selection { get; set; }
    public T[] Items { get; set; }
    public object GridStyle { get; } = gridStyle;
    public object GridItemTemplate { get; } = itemTemplate;

    public override ContentDialog GetDialog(XamlRoot root)
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

      var dialog = base.GetDialog(root);
      dialog.Content = gridView;

      gridView.SelectionChanged += (s, e) => { Selection = (T)gridView.SelectedItem; };

      dialog.Loaded += (sender, e) =>
      {
        var root = VisualTreeHelper.GetParent(dialog);
        var primaryButton = root.FindChildByName("PrimaryButton") as Button;

        // Add event to click the primary button when selected item has been double tapped.
        (dialog.Content as GridView).DoubleTapped += (sender, e) =>
        {
          var PrimaryFeap = FrameworkElementAutomationPeer.FromElement(primaryButton) as ButtonAutomationPeer;
          if (PrimaryFeap != null)
          {
            // Click the primary button
            PrimaryFeap?.Invoke();
          }
          else
          {
            // If primary button is not available, close the dialog
            dialog.Hide();
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
  /// GridViewDialog that has draggable items.
  /// Dragging will close the dialog automaticly.
  /// </summary>
  /// <typeparam name="T">Items type</typeparam>
  public class DraggableGridViewDialog<T>(string title = "", string itemTemplate = "", string gridStyle = "") : GridViewDialog<T>(title, itemTemplate, gridStyle)
  {
    public override ContentDialog GetDialog(XamlRoot root)
    {
      var dialog = base.GetDialog(root);

      var gridview = (dialog.Content as GridView);
      gridview.CanDragItems = true;

      (dialog.Content as GridView).DragItemsStarting += (s, e) => DraggableGridViewDialog_DragItemsStarting(dialog, e);

      return dialog;
    }

    protected virtual void DraggableGridViewDialog_DragItemsStarting(ContentDialog dialog, DragItemsStartingEventArgs e) => dialog.Hide();
  }
  #endregion
}
