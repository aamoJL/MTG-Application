using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.Deck;

public partial class DeckViewModel
{
  public static class Notifications
  {
    public static Notification SaveSuccess => new(NotificationType.Success, "The deck was saved successfully.");
    public static Notification SaveError => new(NotificationType.Error, "Error: Could not save the deck.");

    public static Notification DeleteSuccess => new(NotificationType.Success, "The deck was deleted successfully.");
    public static Notification DeleteError => new(NotificationType.Error, "Error. Could not delete the deck.");
  }
}