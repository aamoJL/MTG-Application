using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.NotificationService;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.UseCaseTests;

[TestClass]
public class SendNotificationTests
{
  [TestMethod]
  public void Execute_NotificationSent()
  {
    var notificationSent = false;
    var notification = new Notification(NotificationType.Success, "Test");

    var notifier = new Notifier()
    {
      OnNotify = (arg) =>
      {
        if (arg == notification) notificationSent = true;
      }
    };

    _ = new SendNotification(notifier).Execute(notification);

    Assert.IsTrue(notificationSent);
  }
}