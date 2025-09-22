using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.TestUtility.Services;

public class NotificationException(Notification notification) : UnitTestAssertException
{
  public Notification Notification { get; } = notification;
}

public class TestNotifier : Notifier
{
  public TestNotifier() => OnNotify = (msg) => Notified = msg;

  public Notification Notified { get; private set; } = null;
}

public static class NotificationAssert
{
  public static void NotificationSent(NotificationType type, TestNotifier notifier)
  {
    if (notifier.Notified == null)
      Assert.Fail("Notification was not sent");

    Assert.IsTrue(notifier.Notified.NotificationType.Equals(type), $"Expected {type}, was {notifier.Notified.NotificationType}");
  }

  public static void NotificationSent(Notification notification, TestNotifier notifier)
  {
    if (notifier.Notified == null)
      Assert.Fail("Notification was not sent");

    Assert.IsTrue(notifier.Notified.Equals(notification), "Notifications do not match");
  }

  public static void NotificationNotSent(NotificationType type, TestNotifier notifier)
    => Assert.AreNotEqual(notifier.Notified.NotificationType, type, $"Should not have been {notifier.Notified.NotificationType}");

  public static void NotificationNotSent(TestNotifier notifier)
    => Assert.IsNull(notifier.Notified, "Notification should not have been sent");
}