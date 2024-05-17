using MTGApplication.General.Services.NotificationService;

namespace MTGApplication.Features.DeckEditor;

public class DeckEditorViewNotifications
{
  public static void RegisterNotifications(DeckEditorNotifier notifier, object sender)
    => notifier.OnNotify = (arg) => { NotificationService.RaiseNotification(sender, arg); };
}