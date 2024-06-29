using Microsoft.UI.Xaml;
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

  [Obsolete("Use Content Dialog")]
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

  [Obsolete("Use Content Dialog")]
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
