using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCard;

[TestClass]
public class DeleteCard
{
  [TestMethod]
  public void Delete()
  {
    var deleteCalled = false;
    var factory = new TestDeckCardViewModelFactory()
    {
      OnCardDelete = _ => deleteCalled = true
    };
    var vm = factory.Build();

    vm.DeleteCardCommand.Execute(null);

    Assert.IsTrue(deleteCalled);
  }

  [TestMethod]
  public void Delete_Exception_NotificationShown()
  {
    var factory = new TestDeckCardViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      OnCardDelete = _ => throw new()
    };
    var vm = factory.Build();

    vm.DeleteCardCommand.Execute(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}
