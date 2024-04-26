using System;

namespace MTGApplication.General.Services.NotificationService;

public static class NotificationService
{
  public enum NotificationType { Info, Error, Warning, Success }

  public record class Notification(NotificationType Type, string Message);

  public class Notifier
  {
    public Action<Notification> OnNotify { private get; set; }

    public void Notify(Notification notification)
      => OnNotify?.Invoke(notification);
  }
}
