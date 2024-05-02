using Microsoft.VisualStudio.TestTools.UnitTesting;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.TestUtility;

public class NotificationException(NotificationType notificationType) : UnitTestAssertException
{
  public NotificationType NotificationType { get; } = notificationType;
}

public static class NotificationAssert
{
  public static async Task NotificationSent(NotificationType notificationType, Task task)
  {
    try { throw await Assert.ThrowsExceptionAsync<NotificationException>(() => task); }
    catch (NotificationException e) { Assert.AreEqual(e.NotificationType, notificationType, "Notification type was wrong"); }
  }
}