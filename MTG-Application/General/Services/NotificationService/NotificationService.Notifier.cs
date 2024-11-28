using System;

namespace MTGApplication.General.Services.NotificationService;

public static partial class NotificationService
{
  public class Notifier
  {
    public Action<Notification>? OnNotify { private get; set; }

    public void Notify(Notification notification)
      => OnNotify?.Invoke(notification);
  }
}
