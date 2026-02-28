using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.TestUtility.Services;

public class NotificationException(Notification notification) : UnitTestAssertException
{
  public override string Message => $"Exception: {Notification.NotificationType} notification was sent.\n Message: {Notification.Message}";
  public Notification Notification { get; } = notification;
}

public class TestNotifier : Notifier
{
  public virtual Notification? Notified { get; protected set; } = null;
  public bool ThrowOnError { get; init; } = true;

  public override void Notify(Notification notification)
  {
    if (ThrowOnError && notification.NotificationType == NotificationType.Error)
      throw new NotificationException(notification);

    Notified = notification;
  }
}

public class NotImplementedNotifier : TestNotifier
{
  public override void Notify(Notification notification)
    => Assert.Fail($"Notifier not implemented: {notification.Message}");
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
    => Assert.AreNotEqual(notifier.Notified?.NotificationType, type, $"Should not have been {notifier.Notified?.NotificationType}");

  public static void NotificationNotSent(TestNotifier notifier)
    => Assert.IsNull(notifier.Notified, "Notification should not have been sent");
}