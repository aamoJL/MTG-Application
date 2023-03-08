using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;

namespace MTGApplication
{
  /// <summary>
  /// Main Window
  /// </summary>
  public sealed partial class MainWindow : Window
  {
    public MainWindow()
    {
      this.InitializeComponent();

      // Change window icon
      IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
      WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
      var appWindow = AppWindow.GetFromWindowId(windowId);
      appWindow.SetIcon("Assets/Icon.ico");
    }
  }
}
