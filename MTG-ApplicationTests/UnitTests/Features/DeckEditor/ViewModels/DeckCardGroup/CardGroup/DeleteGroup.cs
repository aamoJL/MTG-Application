using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;

[TestClass]
public class DeleteGroup
{
  [TestMethod]
  public void Delete_DefaultGroup_CanNotExecute()
  {
    var factory = new TestDeckCardGroupViewModelFactory();
    var vm = factory.Build();

    Assert.IsFalse(vm.DeleteGroupCommand.CanExecute(null));
  }

  [TestMethod]
  public void Delete_HasName_CanExecute()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [])
    };
    var vm = factory.Build();

    Assert.IsTrue(vm.DeleteGroupCommand.CanExecute(null));
  }

  [TestMethod]
  public void Delete()
  {
    var onDeleteCalled = false;
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", []),
      Notifier = new(),
      OnGroupDelete = _ => onDeleteCalled = true
    };
    var vm = factory.Build();

    vm.DeleteGroupCommand.Execute(null);

    Assert.IsTrue(onDeleteCalled);
  }

  [TestMethod]
  public void Delete_Exception_NotificationShown()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", []),
      Notifier = new() { ThrowOnError = false },
      OnGroupDelete = _ => throw new()
    };
    var vm = factory.Build();

    vm.DeleteGroupCommand.Execute(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}
