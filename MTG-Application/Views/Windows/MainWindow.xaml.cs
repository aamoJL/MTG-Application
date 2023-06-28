using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;

namespace MTGApplication;

/// <summary>
/// Main Window
/// </summary>
public sealed partial class MainWindow : Window
{
  public MainWindow()
  {
    this.InitializeComponent();

    // Change window icon
    var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
    var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
    var appWindow = AppWindow.GetFromWindowId(windowId);
    appWindow.SetIcon("Assets/Icon.ico");

    Title = "MTG Application";

    (Content as FrameworkElement).RequestedTheme = AppConfig.LocalSettings.AppTheme;
    AppConfig.LocalSettings.PropertyChanged += LocalSettings_PropertyChanged;
  }

  private void LocalSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
    {
      (Content as FrameworkElement).RequestedTheme = AppConfig.LocalSettings.AppTheme;
    }
  }
}
