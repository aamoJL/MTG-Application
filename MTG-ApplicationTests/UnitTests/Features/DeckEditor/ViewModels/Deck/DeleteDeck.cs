using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.Deck;

[TestClass]
public class DeleteDeck
{
  [TestMethod]
  public async Task Delete_Unnamed_CanNotExecute()
  {
    var factory = new TestDeckViewModelFactory();
    var vm = factory.Build();

    Assert.IsFalse(vm.DeleteDeckCommand.CanExecute(null));
  }

  [TestMethod]
  public async Task Delete_HasName_CanExecute()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Model = new() { Name = "Name" },
    };
    var vm = factory.Build();

    Assert.IsTrue(vm.DeleteDeckCommand.CanExecute(null));
  }

  [TestMethod]
  public async Task Delete_InvalidState_NotificationShown()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
    };
    var vm = factory.Build();

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Delete_Cancel_Return()
  {
    var deleted = false;
    var factory = new TestDeckViewModelFactory()
    {
      Model = new() { Name = "Deck" },
      Confirmers = new()
      {
        ConfirmDeckDelete = (_) => Task.FromResult(ConfirmationResult.Cancel)
      },
      OnDeckDeleted = () => { deleted = true; return Task.CompletedTask; }
    };
    var vm = factory.Build();

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(deleted);
  }

  [TestMethod]
  public async Task Delete_Accept_Deleted()
  {
    var deleted = false;
    var factory = new TestDeckViewModelFactory()
    {
      Model = new() { Name = "Deck" },
      Notifier = new(),
      Repository = new()
      {
        DeleteResult = (_) => Task.FromResult(true)
      },
      Confirmers = new()
      {
        ConfirmDeckDelete = (_) => Task.FromResult(ConfirmationResult.Yes)
      },
      OnDeckDeleted = () => { deleted = true; return Task.CompletedTask; }
    };
    var vm = factory.Build();

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    Assert.IsTrue(deleted);
  }

  [TestMethod]
  public async Task Delete_Success_NotificationShown()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Model = new() { Name = "Deck" },
      Notifier = new(),
      Repository = new()
      {
        DeleteResult = (_) => Task.FromResult(true)
      },
      Confirmers = new()
      {
        ConfirmDeckDelete = (_) => Task.FromResult(ConfirmationResult.Yes)
      },
      OnDeckDeleted = () => Task.CompletedTask
    };
    var vm = factory.Build();

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Success, factory.Notifier);
  }

  [TestMethod]
  public async Task Delete_Failure_NotificationShown()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Model = new() { Name = "Deck" },
      Notifier = new() { ThrowOnError = false },
      Repository = new()
      {
        DeleteResult = (_) => Task.FromResult(false)
      },
      Confirmers = new()
      {
        ConfirmDeckDelete = (_) => Task.FromResult(ConfirmationResult.Yes)
      },
    };
    var vm = factory.Build();

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Delete_Exception_NotificationShown()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Model = new() { Name = "Deck" },
      Notifier = new() { ThrowOnError = false },
      Repository = new()
      {
        DeleteResult = (_) => Task.FromResult(false)
      },
      Confirmers = new()
      {
        ConfirmDeckDelete = (_) => throw new()
      },
    };
    var vm = factory.Build();

    await vm.DeleteDeckCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}
