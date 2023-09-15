using System;
using static MTGApplication.Services.NotificationService;

namespace MTGApplication.Interfaces;

/// <summary>
/// Interface for classes that can raise an in app notification.
/// Views should subscribe to the <see cref="OnNotification"/> event to raise the notification to the <see cref="Services.NotificationService"/>
/// </summary>
public interface IInAppNotifier
{
  /// <summary>
  /// Event, that will be invoked when in app notification should be shown
  /// </summary>
  public event EventHandler<NotificationEventArgs> OnNotification;

  /// <summary>
  /// Shows in app notification on a window
  /// </summary>
  public void RaiseInAppNotification(NotificationType type, string text);
}
