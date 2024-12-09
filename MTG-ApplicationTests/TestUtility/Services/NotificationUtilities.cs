using Microsoft.VisualStudio.TestTools.UnitTesting;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.TestUtility.Services;

public class NotificationException(Notification notification) : UnitTestAssertException
{
  public Notification Notification { get; } = notification;
}

public class TestNotifier : Notifier
{
  public TestNotifier()
    => OnNotify = (msg) => Notified = msg;

  public Notification Notified { get; private set; } = null;
}

public static class NotificationAssert
{
  [Obsolete("Use TestNotifier")]
  public static async Task NotificationSent(NotificationType notificationType, Func<Task> task)
  {
    try { throw await Assert.ThrowsExceptionAsync<NotificationException>(task); }
    catch (NotificationException e) { Assert.AreEqual(notificationType, e.Notification.NotificationType, "Notification type was wrong"); }
  }

  public static void NotificationSent(NotificationType type, TestNotifier notifier)
    => Assert.IsTrue(notifier.Notified?.NotificationType.Equals(type), $"Expected {type}, was {notifier.Notified.NotificationType}");

  public static void NotificationSent(Notification notification, TestNotifier notifier)
    => Assert.IsTrue(notifier.Notified?.Equals(notification), "Notifications do not match");

  public static void NotificationNotSent(NotificationType type, TestNotifier notifier)
    => Assert.IsFalse(notifier.Notified?.NotificationType.Equals(type), $"Should not have been {notifier.Notified.NotificationType}");
}