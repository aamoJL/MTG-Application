using System;

namespace MTGApplication.General.Services.NotificationService;

public static class NotificationService
{
  public enum NotificationType { Info, Error, Warning, Success }

  public record class Notification(NotificationType NotificationType, string Message);

  public static event EventHandler<Notification> OnShow;

  /// <summary>
  /// Notification duration in milliseconds
  /// </summary>
  public static int NotificationDuration => 5000;

  public static void RaiseNotification(object sender, Notification notification) 
    => OnShow?.Invoke(sender, notification);

  public class Notifier
  {
    public Action<Notification> OnNotify { private get; set; }

    public void Notify(Notification notification)
      => OnNotify?.Invoke(notification);
  }
}
