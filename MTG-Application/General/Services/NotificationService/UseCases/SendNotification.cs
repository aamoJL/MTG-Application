using MTGApplication.General.ViewModels;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.General.Services.NotificationService.UseCases;

public class SendNotification(Notifier notifier) : UseCase<Notification, bool>
{
  public override bool Execute(Notification notification)
  {
    notifier.Notify(notification);
    return true;
  }
}