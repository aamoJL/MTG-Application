using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Views.Dialogs.Controls;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.ConfirmationService;

/// <summary>
/// Service to show dialogs on windows.
/// </summary>
public static class DialogService
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
  }
}
