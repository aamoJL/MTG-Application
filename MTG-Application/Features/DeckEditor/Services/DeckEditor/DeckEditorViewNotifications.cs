using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor;

public class DeckEditorViewNotifications
{
  public static void RegisterNotifications(Notifier notifier, object sender)
    => notifier.OnNotify = (arg) => { RaiseNotification(sender, arg); };
}