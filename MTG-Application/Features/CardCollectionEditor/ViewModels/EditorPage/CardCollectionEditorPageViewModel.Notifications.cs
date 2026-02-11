using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.EditorPage;

public partial class CardCollectionEditorPageViewModel
{
  public static class Notifications
  {
    public static Notification OpenCollectionSuccess => new(NotificationType.Success, "The collection was loaded successfully.");
    public static Notification OpenCollectionError => new(NotificationType.Error, "Error. Could not load the collection.");
  }
}