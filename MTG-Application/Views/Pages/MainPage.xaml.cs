using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MTGApplication.Services;
using System.Windows.Input;
using Windows.UI;

namespace MTGApplication.Pages;

/// <summary>
/// Code behind for MainPage
/// </summary>
public sealed partial class MainPage : Page
{
  private readonly int notificationDuration = 5000;

  public ICommand ChangeThemeCommand { get; }

  public MainPage()
  {
    this.InitializeComponent();

    ChangeThemeCommand = new RelayCommand(() =>
    {
      var currentTheme = (App.MainWindow.Content as FrameworkElement).RequestedTheme;
      AppConfig.LocalSettings.AppTheme = currentTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
    });

    NotificationService.OnNotification += Notifications_OnNotification;
  }

  private void Notifications_OnNotification(object sender, NotificationService.NotificationEventArgs e)
  {
    PopupAppNotification.Background = e.Type switch
    {
      NotificationService.NotificationType.Error => new SolidColorBrush(Color.FromArgb(255, 248, 215, 218)),
      NotificationService.NotificationType.Warning => new SolidColorBrush(Color.FromArgb(255, 255, 243, 205)),
      NotificationService.NotificationType.Success => new SolidColorBrush(Color.FromArgb(255, 212, 237, 218)),
      _ => new SolidColorBrush(Color.FromArgb(255, 204, 229, 255)),
    };
    PopupAppNotification.RequestedTheme = Microsoft.UI.Xaml.ElementTheme.Light;
    PopupAppNotification.Show(e.Text, notificationDuration);
  }
}
