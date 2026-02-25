using System;

namespace MTGApplication.General.Services.NotificationService;

public static partial class NotificationService
{
  public class Notifier
  {
    public event EventHandler<Notification>? OnNotifyEvent;

    public virtual void Notify(Notification notification) => OnNotifyEvent?.Invoke(this, notification);
  }
}
