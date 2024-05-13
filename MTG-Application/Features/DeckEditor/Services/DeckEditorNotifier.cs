using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor;

public class DeckEditorNotifier : Notifier
{
  public class DeckEditorNotifications
  {
    // Load
    public static Notification LoadSuccessNotification => new(NotificationType.Success, "The deck was loaded successfully.");
    public static Notification LoadErrorNotification => new(NotificationType.Error, "Error. Could not load the deck.");
    // Save
    public static Notification SaveSuccessNotification => new(NotificationType.Success, "The deck was saved successfully.");
    public static Notification SaveErrorNotification => new(NotificationType.Error, "Error: Could not save the deck.");
    // Delete
    public static Notification DeleteSuccessNotification => new(NotificationType.Success, "The deck was deleted successfully.");
    public static Notification DeleteErrorNotification => new(NotificationType.Error, "Error. Could not delete the deck.");
  }

  public DeckEditorNotifications Notifications { get; set; } = new();
}
