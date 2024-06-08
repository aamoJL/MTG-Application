namespace MTGApplication.General.Services.NotificationService;

public static partial class NotificationService
{
  public record class Notification(NotificationType NotificationType, string Message);
}
