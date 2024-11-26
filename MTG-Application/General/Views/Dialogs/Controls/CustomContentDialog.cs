using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Views.Extensions;
using System;
using System.Threading.Tasks;

namespace MTGApplication.General.Views.Dialogs.Controls;

public abstract partial class CustomContentDialog<T> : ContentDialog
{
  public CustomContentDialog(string title)
  {
    Title = title;

    PrimaryButtonText = "Yes";
    SecondaryButtonText = "No";
    CloseButtonText = "Cancel";

    RequestedTheme = AppConfig.LocalSettings.AppTheme;
    DefaultButton = ContentDialogButton.Primary;

    Loaded += CustomContentDialog_Loaded;
  }

  public new async Task<T> ShowAsync() => ProcessResult(await base.ShowAsync());

  protected abstract T ProcessResult(ContentDialogResult result);

  private void CustomContentDialog_Loaded(object sender, RoutedEventArgs e)
  {
    Loaded -= CustomContentDialog_Loaded;

    SetBackgroundClickDismiss();
    SetPrimaryButtonStyle();
  }

  private void SetBackgroundClickDismiss()
  {
    var root = VisualTreeHelper.GetParent(this);
    var smokeLayer = root.FindChildByName("SmokeLayerBackground") as FrameworkElement;
    var pressed = false;

    if (smokeLayer != null)
    {
      smokeLayer.PointerPressed += (sender, e) => pressed = true;
      smokeLayer.PointerReleased += (sender, e) =>
      {
        if (pressed == true)
          Hide();

        pressed = false;
      };
    }
  }

  private void SetPrimaryButtonStyle()
  {
    Application.Current.Resources.TryGetValue("PrimaryButtonStyle", out var pbs);

    if (pbs is Style primaryButtonStyle)
      PrimaryButtonStyle = primaryButtonStyle;
  }
}

public abstract class BasicDialog(string title) : CustomContentDialog<ContentDialogResult>(title);

public abstract class ConfirmationDialog(string title) : CustomContentDialog<ConfirmationResult>(title);

public abstract class StringDialog(string title) : CustomContentDialog<string>(title);

public abstract class ConfirmationDialogWithBool(string title) : CustomContentDialog<(ConfirmationResult, bool)>(title);

public abstract class ObjectDialog(string title) : CustomContentDialog<object>(title);

public abstract class StringStringDialog(string title) : CustomContentDialog<(string, string)>(title);
