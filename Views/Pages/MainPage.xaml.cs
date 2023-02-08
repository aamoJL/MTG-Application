using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace MTGApplication.Pages
{
  /// <summary>
  /// Main Page
  /// </summary>
  public sealed partial class MainPage : Page
  {
    private readonly int notificationDuration = 4000;
    
    public MainPage()
    {
      this.InitializeComponent();

      Notifications.OnNotification += Notifications_OnNotification;
    }

    private void Notifications_OnNotification(object sender, Notifications.NotificationEventArgs e)
    {
      switch (e.Type)
      {
        case Notifications.NotificationType.Error:
          PopupAppNotification.Background = new SolidColorBrush(Color.FromArgb(255, 248, 215, 218)); break;
        case Notifications.NotificationType.Warning:
          PopupAppNotification.Background = new SolidColorBrush(Color.FromArgb(255, 255, 243, 205)); break;
        case Notifications.NotificationType.Success:
          PopupAppNotification.Background = new SolidColorBrush(Color.FromArgb(255, 212, 237, 218)); break;
        case Notifications.NotificationType.Info:
        default: 
          PopupAppNotification.Background = new SolidColorBrush(Color.FromArgb(255, 204, 229, 255)); break;
      }

      PopupAppNotification.Show(e.Text, notificationDuration);
    }
  }
}
