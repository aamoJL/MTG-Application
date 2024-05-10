using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor;

public class DeckEditorNotifier : Notifier
{
  public class DeckEditorNotifications
  {
    // Load
    public Notification LoadSuccessNotification => new(NotificationType.Success, "The deck was loaded successfully.");
    public Notification LoadErrorNotification => new(NotificationType.Error, "Error. Could not load the deck.");
    // Save
    public Notification SaveSuccessNotification => new(NotificationType.Success, "The deck was saved successfully.");
    public Notification SaveErrorNotification => new(NotificationType.Error, "Error: Could not save the deck.");
    // Delete
    public Notification DeleteSuccessNotification => new(NotificationType.Success, "The deck was deleted successfully.");
    public Notification DeleteErrorNotification => new(NotificationType.Error, "Error. Could not delete the deck.");
  }

  public DeckEditorNotifications Notifications { get; set; } = new();
}
