using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MTGApplication.Services;
using Windows.UI;

namespace MTGApplication.Pages
{
    /// <summary>
    /// Main Page
    /// </summary>
    public sealed partial class MainPage : Page
  {
    private readonly int notificationDuration = 5000;
    
    public MainPage()
    {
      this.InitializeComponent();

      Notifications.OnNotification += Notifications_OnNotification;
    }

    private void Notifications_OnNotification(object sender, Notifications.NotificationEventArgs e)
    {
      PopupAppNotification.Background = e.Type switch
      {
        Notifications.NotificationType.Error => new SolidColorBrush(Color.FromArgb(255, 248, 215, 218)),
        Notifications.NotificationType.Warning => new SolidColorBrush(Color.FromArgb(255, 255, 243, 205)),
        Notifications.NotificationType.Success => new SolidColorBrush(Color.FromArgb(255, 212, 237, 218)),
        _ => new SolidColorBrush(Color.FromArgb(255, 204, 229, 255)),
      };
      PopupAppNotification.Show(e.Text, notificationDuration);
    }
  }
}
