using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.TestUtility.Services;

public class NotificationException(Notification notification) : UnitTestAssertException
{
  public Notification Notification { get; } = notification;
}

public static class NotificationAssert
{
  public static void NotificationSent(NotificationType notificationType, Action action)
  {
    try { throw Assert.ThrowsException<NotificationException>(action); }
    catch (NotificationException e) { Assert.AreEqual(notificationType, e.Notification.NotificationType, "Notification type was wrong"); }
  }

  public static async Task NotificationSent(NotificationType notificationType, Func<Task> task)
  {
    try { throw await Assert.ThrowsExceptionAsync<NotificationException>(task); }
    catch (NotificationException e) { Assert.AreEqual(notificationType, e.Notification.NotificationType, "Notification type was wrong"); }
  }

  public static async Task NotificationSent(Notification notification, Func<Task> task)
  {
    try { throw await Assert.ThrowsExceptionAsync<NotificationException>(task); }
    catch (NotificationException e) { Assert.AreEqual(notification, e.Notification, "Notification was wrong"); }
  }
}