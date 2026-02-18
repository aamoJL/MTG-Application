using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.Collection;

[TestClass]
public class DeleteCollection
{
  [TestMethod]
  public async Task Delete_Unnamed_CanNotExecute()
  {
    var model = new MTGCardCollection()
    {
      Name = string.Empty
    };
    var factory = new TestCollectionViewModelFactory();
    var vm = factory.Build(model);

    Assert.IsFalse(vm.DeleteCollectionCommand.CanExecute(null));
  }

  [TestMethod]
  public async Task Delete_HasName_CanExecute()
  {
    var model = new MTGCardCollection()
    {
      Name = "Name"
    };
    var factory = new TestCollectionViewModelFactory();
    var vm = factory.Build(model);

    Assert.IsTrue(vm.DeleteCollectionCommand.CanExecute(null));
  }

  [TestMethod]
  public async Task Delete_InvalidState_NotificationShown()
  {
    var model = new MTGCardCollection()
    {
      Name = string.Empty
    };
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
    };
    var vm = factory.Build(model);

    await vm.DeleteCollectionCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Delete_Cancel_Return()
  {
    var deleted = false;
    var model = new MTGCardCollection()
    {
      Name = "Collection",
    };
    var factory = new TestCollectionViewModelFactory()
    {
      CollectionConfirmers = new()
      {
        ConfirmCollectionDelete = (_) => Task.FromResult(ConfirmationResult.Cancel)
      },
      OnCollectionDeleted = () => { deleted = true; return Task.CompletedTask; }
    };
    var vm = factory.Build(model);

    await vm.DeleteCollectionCommand.ExecuteAsync(null);

    Assert.IsFalse(deleted);
  }

  [TestMethod]
  public async Task Delete_Accept_Deleted()
  {
    var deleted = false;
    var model = new MTGCardCollection()
    {
      Name = "Collection",
    };
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new(),
      Repository = new()
      {
        DeleteResult = (_) => Task.FromResult(true)
      },
      CollectionConfirmers = new()
      {
        ConfirmCollectionDelete = (_) => Task.FromResult(ConfirmationResult.Yes)
      },
      OnCollectionDeleted = () => { deleted = true; return Task.CompletedTask; }
    };
    var vm = factory.Build(model);

    await vm.DeleteCollectionCommand.ExecuteAsync(null);

    Assert.IsTrue(deleted);
  }

  [TestMethod]
  public async Task Delete_Success_NotificationShown()
  {
    var model = new MTGCardCollection()
    {
      Name = "Collection",
    };
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new(),
      Repository = new()
      {
        DeleteResult = (_) => Task.FromResult(true)
      },
      CollectionConfirmers = new()
      {
        ConfirmCollectionDelete = (_) => Task.FromResult(ConfirmationResult.Yes)
      },
      OnCollectionDeleted = null
    };
    var vm = factory.Build(model);

    await vm.DeleteCollectionCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Success, factory.Notifier);
  }

  [TestMethod]
  public async Task Delete_Failure_NotificationShown()
  {
    var model = new MTGCardCollection()
    {
      Name = "Collection",
    };
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      Repository = new()
      {
        DeleteResult = (_) => Task.FromResult(false)
      },
      CollectionConfirmers = new()
      {
        ConfirmCollectionDelete = (_) => Task.FromResult(ConfirmationResult.Yes)
      },
    };
    var vm = factory.Build(model);

    await vm.DeleteCollectionCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Delete_Exception_NotificationShown()
  {
    var model = new MTGCardCollection()
    {
      Name = "Collection",
    };
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      Repository = new()
      {
        DeleteResult = (_) => Task.FromResult(false)
      },
      CollectionConfirmers = new()
      {
        ConfirmCollectionDelete = (_) => throw new()
      },
    };
    var vm = factory.Build(model);

    await vm.DeleteCollectionCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}