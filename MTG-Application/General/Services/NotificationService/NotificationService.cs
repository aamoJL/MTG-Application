using System;

namespace MTGApplication.General.Services.NotificationService;

public static partial class NotificationService
{
  public enum NotificationType { Info, Error, Warning, Success }

  /// <summary>
  /// Notification duration in milliseconds
  /// </summary>
  public static int NotificationDuration => 5000;

  public static event EventHandler<Notification>? OnShow;

  [Obsolete("Use event")]
  public static void RegisterNotifications(Notifier notifier, object sender)
    => notifier.OnNotify = (arg) => RaiseNotification(sender, arg);

  /// <summary>
  /// Raises UI notification
  /// </summary>
  /// <param name="sender">Page or other UI element that has a same XamlRoot as the window that will shot the notification</param>
  /// <param name="notification">Notification information</param>
  public static void RaiseNotification(object sender, Notification notification)
    => OnShow?.Invoke(sender, notification);
}
