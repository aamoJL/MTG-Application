using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Views.Dialogs.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.ConfirmationService;

/// <summary>
/// Service to show dialogs on windows.
/// </summary>
public static class DialogService
{
  public static Dictionary<XamlRoot, ContentDialog> CurrentDialogs { get; } = [];

  public static async Task<T?> ShowAsync<T>(XamlRoot root, CustomContentDialog<T> dialog, bool force = false)
  {
    dialog.XamlRoot = root;

    // Only one dialog can be open on a window
    // Return default or force the current dialog to close if exists
    if (CurrentDialogs.GetValueOrDefault(root) is ContentDialog currentDialog)
    {
      if (force)
        currentDialog.Hide();
      else
        return default;
    }

    CurrentDialogs[root] = dialog;

    var result = await dialog.ShowAsync();

    // Remove dialog from dialogs if the result was from the current dialog
    // Prevents crash if user tries to open multiple dialogs on a window
    if (CurrentDialogs.GetValueOrDefault(root) == dialog)
      CurrentDialogs.Remove(root);

    return result;
  }
}
