using MTGApplication.General.ViewModels;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor;

public class SendNotification : UseCase<Notification, bool>
{
  public SendNotification(Notifier notifier) => Notifier = notifier;

  public Notifier Notifier { get; }

  public override bool Execute(Notification notification)
  {
    Notifier.Notify(notification);
    return true;
  }
}