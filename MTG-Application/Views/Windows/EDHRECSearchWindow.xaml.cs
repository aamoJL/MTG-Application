using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using MTGApplication.Models.Structs;

namespace MTGApplication.Views.Windows;

/// <summary>
/// Window that has EDHREC card search
/// </summary>
public sealed partial class EDHRECSearchWindow : Window
{
  public EDHRECSearchWindow(CommanderTheme[] themes)
  {
    this.InitializeComponent();

    // Change window icon
    var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
    var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
    var appWindow = AppWindow.GetFromWindowId(windowId);
    appWindow.SetIcon("Assets/Icon.ico");

    appWindow.Resize(new(550, appWindow.Size.Height));

    Title = "Deck Testing";
    CommanderThemes = themes;

    (Content as FrameworkElement).RequestedTheme = AppConfig.LocalSettings.AppTheme;
    AppConfig.LocalSettings.PropertyChanged += LocalSettings_PropertyChanged;

    Closed += DeckTestingWindow_Closed;
  }

  public CommanderTheme[] CommanderThemes { get; }

  private void DeckTestingWindow_Closed(object sender, WindowEventArgs args) => AppConfig.LocalSettings.PropertyChanged -= LocalSettings_PropertyChanged;

  private void LocalSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
    {
      (Content as FrameworkElement).RequestedTheme = AppConfig.LocalSettings.AppTheme;
    }
  }
}
