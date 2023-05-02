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
  }
}
