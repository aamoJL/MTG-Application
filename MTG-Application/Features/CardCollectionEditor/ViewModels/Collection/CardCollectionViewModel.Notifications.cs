using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.Collection;

public partial class CardCollectionViewModel
{
  public static class Notifications
  {
    public static Notification SaveCollectionSuccess => new(NotificationType.Success, "The list was saved successfully.");
    public static Notification SaveCollectionError => new(NotificationType.Error, "Error. Could not save the collection.");

    public static Notification DeleteCollectionSuccess => new(NotificationType.Success, "The list was deleted successfully.");
    public static Notification DeleteCollectionError => new(NotificationType.Error, "Error. Could not delete the list.");

    public static Notification NewListSuccess => new(NotificationType.Success, "The list was added to the collection successfully.");
    public static Notification NewListNameError => new(NotificationType.Error, "Error. Name can't be empty.");
    public static Notification NewListQueryError => new(NotificationType.Error, "Error. Search query can't be empty.");
    public static Notification NewListExistsError => new(NotificationType.Error, "Error. List already exists in the collection.");

    public static Notification DeleteListSuccess => new(NotificationType.Success, "The list was deleted successfully.");
  }
}
