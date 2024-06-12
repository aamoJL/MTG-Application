using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.Editor.Services;

public static class DeckEditorNotifications
{
  public static Notification LoadSuccess => new(NotificationType.Success, "The deck was loaded successfully.");
  public static Notification LoadError => new(NotificationType.Error, "Error. Could not load the deck.");

  public static Notification SaveSuccess => new(NotificationType.Success, "The deck was saved successfully.");
  public static Notification SaveError => new(NotificationType.Error, "Error: Could not save the deck.");

  public static Notification DeleteSuccess => new(NotificationType.Success, "The deck was deleted successfully.");
  public static Notification DeleteError => new(NotificationType.Error, "Error. Could not delete the deck.");
}