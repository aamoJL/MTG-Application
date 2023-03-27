using System;

namespace MTGApplication.Services;

/// <summary>
/// Service that handles application notifications
/// </summary>
public static class NotificationService
{
  public enum NotificationType { Info, Error, Warning, Success }

  public class NotificationEventArgs : EventArgs
  {
    public NotificationType Type { get; set; }
    public string Text { get; set; }

    public NotificationEventArgs(NotificationType type, string text)
    {
      Type = type;
      Text = text;
    }
  }

  public static event EventHandler<NotificationEventArgs> OnNotification;

  /// <summary>
  /// Sends notification
  /// </summary>
  public static void RaiseNotification(NotificationType type, string text) => OnNotification?.Invoke(null, new NotificationEventArgs(type, text));
}