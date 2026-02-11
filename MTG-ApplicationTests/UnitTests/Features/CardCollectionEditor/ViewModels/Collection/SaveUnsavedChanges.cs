using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.Collection;

[TestClass]
public class SaveUnsavedChanges
{
  [TestMethod]
  public async Task Save_IsNotDirty_Return()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory();
    var vm = factory.Build(model);

    var saveArgs = new ISavable.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsFalse(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsFalse(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_Cancelled_Return()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true }
    };
    var vm = factory.Build(model);

    var saveArgs = new ISavable.ConfirmArgs() { Cancelled = true };

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsTrue(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_ArgsIsNull_Return()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true }
    };
    var vm = factory.Build(model);

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(null);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Save_Cancel_Cancelled_IsDirty()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      CollectionConfirmers = new()
      {
        ConfirmUnsavedChanges = (_) => Task.FromResult(ConfirmationResult.Cancel)
      }
    };
    var vm = factory.Build(model);

    var saveArgs = new ISavable.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsTrue(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_Decline_Return_IsDirty()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      CollectionConfirmers = new()
      {
        ConfirmUnsavedChanges = (_) => Task.FromResult(ConfirmationResult.No)
      }
    };
    var vm = factory.Build(model);

    var saveArgs = new ISavable.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsFalse(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_CancelSave_Cancelled_IsDirty()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      CollectionConfirmers = new()
      {
        ConfirmUnsavedChanges = (_) => Task.FromResult(ConfirmationResult.Yes),
        ConfirmCollectionSave = (_) => Task.FromResult<string>(null),
      }
    };
    var vm = factory.Build(model);

    var saveArgs = new ISavable.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsTrue(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_Save_IsNotDirty()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(false),
        AddResult = (_) => Task.FromResult(true)
      },
      CollectionConfirmers = new()
      {
        ConfirmUnsavedChanges = (_) => Task.FromResult(ConfirmationResult.Yes),
        ConfirmCollectionSave = (_) => Task.FromResult("New Name"),
      }
    };
    var vm = factory.Build(model);

    var saveArgs = new ISavable.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsFalse(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsFalse(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_Failure_Cancelled_IsDirty()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(false),
        AddResult = (_) => Task.FromResult(false)
      },
      CollectionConfirmers = new()
      {
        ConfirmUnsavedChanges = (_) => Task.FromResult(ConfirmationResult.Yes),
        ConfirmCollectionSave = (_) => Task.FromResult("New Name"),
      }
    };
    var vm = factory.Build(model);

    var saveArgs = new ISavable.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsTrue(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_Exception_Cancelled()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      CollectionConfirmers = new()
      {
        ConfirmUnsavedChanges = (_) => throw new()
      }
    };
    var vm = factory.Build(model);

    var saveArgs = new ISavable.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsTrue(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task IsDirty_Exception_NotificationShown()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      CollectionConfirmers = new()
      {
        ConfirmUnsavedChanges = (_) => throw new()
      }
    };
    var vm = factory.Build(model);

    var saveArgs = new ISavable.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}