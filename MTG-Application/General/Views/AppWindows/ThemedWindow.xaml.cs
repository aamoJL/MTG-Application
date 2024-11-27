using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using MTGApplication.General.Services.NotificationService;
using System;
using Windows.UI;

namespace MTGApplication.General.Views.AppWindows;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public partial class ThemedWindow : Window
{
  private static readonly string DEFAULT_ICON_URI = "Assets/Icon.ico";

  private bool CanClose { get; set; } = false;

  public ThemedWindow() : base()
  {
    InitializeComponent();

    IconUri = DEFAULT_ICON_URI;

    Closed += ThemedWindow_Closed;
    AppConfig.LocalSettings.PropertyChanged += LocalSettings_PropertyChanged;
    NotificationService.OnShow += NotificationService_OnShow;

    if (Content is FrameworkElement element)
      element.RequestedTheme = AppConfig.LocalSettings.AppTheme;
  }

  public string IconUri { set => AppWindow.SetIcon(value); }
  /// <summary>
  /// Window's width
  /// </summary>
  public int Width
  {
    get => AppWindow.Size.Width;
    set => AppWindow.Resize(new(value, Height));
  }
  /// <summary>
  /// Window's height
  /// </summary>
  public int Height
  {
    get => AppWindow.Size.Height;
    set => AppWindow.Resize(new(Width, value));
  }

  public bool Navigate(Type pageType, object? parameters = null) => MainFrame.Navigate(pageType, parameters);

  private void NotificationService_OnShow(object? sender, NotificationService.Notification e)
  {
    if ((sender as FrameworkElement)?.XamlRoot != Content.XamlRoot)
      return;

    InAppNotification.Background = e.NotificationType switch
    {
      NotificationService.NotificationType.Error => new SolidColorBrush(Color.FromArgb(255, 248, 215, 218)),
      NotificationService.NotificationType.Warning => new SolidColorBrush(Color.FromArgb(255, 255, 243, 205)),
      NotificationService.NotificationType.Success => new SolidColorBrush(Color.FromArgb(255, 212, 237, 218)),
      _ => new SolidColorBrush(Color.FromArgb(255, 204, 229, 255)),
    };
    InAppNotification.RequestedTheme = ElementTheme.Light;
    InAppNotification.Show(e.Message, NotificationService.NotificationDuration);
  }

  private void LocalSettings_PropertyChanged(object? _, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (Content is not FrameworkElement element)
      return;

    if (e.PropertyName == nameof(AppConfig.LocalSettings.AppTheme))
      element.RequestedTheme = AppConfig.LocalSettings.AppTheme;
  }

  private async void ThemedWindow_Closed(object? _, WindowEventArgs e)
  {
    // The Window will close if the CanClose variable has been set to true.
    // Otherwise the user will be asked to save unsaved changes.
    // If the user does not cancel the closing event, this method will be called again with the close variable set to true.
    if (CanClose)
      return;

    e.Handled = true;

    if (CanClose = await new WindowClosing(Content.XamlRoot).Close())
    {
      e.Handled = false;
      OnClose();
      Close();
    }
  }

  private void OnClose()
  {
    AppConfig.LocalSettings.PropertyChanged -= LocalSettings_PropertyChanged;
    NotificationService.OnShow -= NotificationService_OnShow;
    Closed -= ThemedWindow_Closed;
    Content = null;
  }
}