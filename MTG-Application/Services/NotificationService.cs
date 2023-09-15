using Microsoft.UI.Xaml;
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

  /// <summary>
  /// Notification duration in milliseconds
  /// </summary>
  public static int NotificationDuration => 5000;

  public static event EventHandler<NotificationEventArgs> OnNotification;

  /// <summary>
  /// Invokes <see cref="OnNotification"/> event
  /// </summary>
  public static void RaiseNotification(XamlRoot root, NotificationEventArgs args) => OnNotification?.Invoke(root, args);
}