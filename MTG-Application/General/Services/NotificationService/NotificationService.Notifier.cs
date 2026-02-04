using System;

namespace MTGApplication.General.Services.NotificationService;

public static partial class NotificationService
{
  public class Notifier
  {
    // TODO: remove
    [Obsolete("Use event")]
    public Action<Notification>? OnNotify { private get; set; }
    public event EventHandler<Notification>? OnNotifyEvent;

    public void Notify(Notification notification)
    {
      // TODO: remove
      OnNotify?.Invoke(notification);
      OnNotifyEvent?.Invoke(this, notification);
    }
  }
}
