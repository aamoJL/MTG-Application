using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Windowing;
using MTGApplication.Models;

namespace MTGApplication.Views.Windows;

public sealed partial class DeckTestingWindow : Window
{
  public DeckTestingWindow(MTGCardDeck cardDeck)
  {
    this.InitializeComponent();

    // Change window icon
    var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
    var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
    var appWindow = AppWindow.GetFromWindowId(windowId);
    appWindow.SetIcon("Assets/Icon.ico");
    
    Title = "Deck Testing";
    CardDeck = cardDeck;

    (Content as FrameworkElement).RequestedTheme = AppConfig.LocalSettings.AppTheme;
    AppConfig.LocalSettings.PropertyChanged += LocalSettings_PropertyChanged;

    Closed += DeckTestingWindow_Closed;
  }

  public MTGCardDeck CardDeck { get; }

  private void DeckTestingWindow_Closed(object sender, WindowEventArgs args) => AppConfig.LocalSettings.PropertyChanged -= LocalSettings_PropertyChanged;

  private void LocalSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
    {
      (Content as FrameworkElement).RequestedTheme = AppConfig.LocalSettings.AppTheme;
    }
  }
}
