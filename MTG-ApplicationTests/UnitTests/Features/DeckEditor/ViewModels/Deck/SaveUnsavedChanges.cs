using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.Deck;

[TestClass]
public class SaveUnsavedChanges
{
  [TestMethod]
  public async Task Save_IsNotDirty_Return()
  {

    var factory = new TestDeckViewModelFactory();
    var vm = factory.Build();

    var saveArgs = new SaveStatus.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsFalse(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsFalse(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_Cancelled_Return()
  {

    var factory = new TestDeckViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true }
    };
    var vm = factory.Build();

    var saveArgs = new SaveStatus.ConfirmArgs() { Cancelled = true };

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsTrue(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_ArgsIsNull_Return()
  {

    var factory = new TestDeckViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true }
    };
    var vm = factory.Build();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(null);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Save_Cancel_Cancelled_IsDirty()
  {
    var factory = new TestDeckViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      Confirmers = new()
      {
        ConfirmUnsavedChanges = (_) => Task.FromResult(ConfirmationResult.Cancel)
      }
    };
    var vm = factory.Build();

    var saveArgs = new SaveStatus.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsTrue(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_Decline_Return_IsDirty()
  {

    var factory = new TestDeckViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      Confirmers = new()
      {
        ConfirmUnsavedChanges = (_) => Task.FromResult(ConfirmationResult.No)
      }
    };
    var vm = factory.Build();

    var saveArgs = new SaveStatus.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsFalse(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_CancelSave_Cancelled_IsDirty()
  {

    var factory = new TestDeckViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      Confirmers = new()
      {
        ConfirmUnsavedChanges = (_) => Task.FromResult(ConfirmationResult.Yes),
        ConfirmDeckSave = (_) => Task.FromResult<string?>(null),
      }
    };
    var vm = factory.Build();

    var saveArgs = new SaveStatus.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsTrue(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_Success_IsNotDirty()
  {

    var factory = new TestDeckViewModelFactory()
    {
      Notifier = new(),
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(false),
        AddResult = (_) => Task.FromResult(true)
      },
      Confirmers = new()
      {
        ConfirmUnsavedChanges = (_) => Task.FromResult(ConfirmationResult.Yes),
        ConfirmDeckSave = (_) => Task.FromResult<string?>("New Name"),
      }
    };
    var vm = factory.Build();

    var saveArgs = new SaveStatus.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsFalse(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsFalse(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_Failure_Cancelled_IsDirty()
  {

    var factory = new TestDeckViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(false),
        AddResult = (_) => Task.FromResult(false)
      },
      Confirmers = new()
      {
        ConfirmUnsavedChanges = (_) => Task.FromResult(ConfirmationResult.Yes),
        ConfirmDeckSave = (_) => Task.FromResult<string?>("New Name"),
      }
    };
    var vm = factory.Build();

    var saveArgs = new SaveStatus.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsTrue(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_Exception_Cancelled()
  {

    var factory = new TestDeckViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      SaveStatus = new() { HasUnsavedChanges = true },
      Confirmers = new()
      {
        ConfirmUnsavedChanges = (_) => throw new()
      }
    };
    var vm = factory.Build();

    var saveArgs = new SaveStatus.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
    Assert.IsTrue(saveArgs.Cancelled);
  }

  [TestMethod]
  public async Task Save_Exception_NotificationShown()
  {

    var factory = new TestDeckViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      SaveStatus = new() { HasUnsavedChanges = true },
      Confirmers = new()
      {
        ConfirmUnsavedChanges = (_) => throw new()
      }
    };
    var vm = factory.Build();

    var saveArgs = new SaveStatus.ConfirmArgs();

    await vm.SaveUnsavedChangesCommand.ExecuteAsync(saveArgs);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}
