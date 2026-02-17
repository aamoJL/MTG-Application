using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.DeckEditor.ViewModels.EditorPage;

public partial class DeckEditorPageViewModel
{
  public static class Notifications
  {
    public static Notification LoadSuccess => new(NotificationType.Success, "The deck was loaded successfully.");
    public static Notification LoadError => new(NotificationType.Error, "Error. Could not load the deck.");
  }
}