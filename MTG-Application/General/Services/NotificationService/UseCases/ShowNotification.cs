using MTGApplication.General.ViewModels;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.General.Services.NotificationService.UseCases;

public class ShowNotification(Notifier notifier) : UseCaseAction<Notification>
{
  public override void Execute(Notification notification)
    => notifier.Notify(notification);
}