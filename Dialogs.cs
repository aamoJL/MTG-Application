using System;

namespace MTGApplication
{
  // TODO: move to service?
  public static class Notifications
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

    public static void RaiseNotification(NotificationType type, string text) => OnNotification?.Invoke(null, new NotificationEventArgs(type, text));
  }
}
