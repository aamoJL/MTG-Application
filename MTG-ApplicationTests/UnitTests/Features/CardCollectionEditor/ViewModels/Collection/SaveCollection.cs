using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.Collection;

[TestClass]
public class SaveCollection
{
  [TestMethod]
  public async Task Save_NameInConfirmer()
  {
    var name = string.Empty;
    var model = new MTGCardCollection()
    {
      Name = "Name"
    };
    var factory = new TestCollectionViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      CollectionConfirmers = new()
      {
        ConfirmCollectionSave = async (data) => { name = data.Data; return await Task.FromResult<string?>(null); },
      }
    };
    var vm = factory.Build(model);

    await vm.SaveCollectionCommand.ExecuteAsync(null);

    Assert.AreEqual("Name", name);
  }

  [TestMethod]
  public async Task Save_Cancel_IsDirty()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      CollectionConfirmers = new()
      {
        ConfirmCollectionSave = (_) => Task.FromResult<string?>(null),
      }
    };
    var vm = factory.Build(model);

    await vm.SaveCollectionCommand.ExecuteAsync(null);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Save_InvalidName_IsDirty()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      SaveStatus = new() { HasUnsavedChanges = true },
      CollectionConfirmers = new()
      {
        ConfirmCollectionSave = (_) => Task.FromResult<string?>(string.Empty),
      }
    };
    var vm = factory.Build(model);

    await vm.SaveCollectionCommand.ExecuteAsync(null);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Save_NewName_NameChanged_IsNotDirty()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new(),
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(false),
        AddResult = (_) => Task.FromResult(true),
      },
      CollectionConfirmers = new()
      {
        ConfirmCollectionSave = (_) => Task.FromResult<string?>("New Name"),
      }
    };
    var vm = factory.Build(model);

    await vm.SaveCollectionCommand.ExecuteAsync(null);

    Assert.IsFalse(vm.SaveStatus.HasUnsavedChanges);
    Assert.AreEqual("New Name", vm.CollectionName);
  }

  [TestMethod]
  public async Task Save_SameName_IsNotDirty()
  {
    var model = new MTGCardCollection()
    {
      Name = "Name"
    };
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new(),
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(false),
        AddResult = (_) => Task.FromResult(true),
      },
      CollectionConfirmers = new()
      {
        ConfirmCollectionSave = (_) => Task.FromResult<string?>("Name"),
      }
    };
    var vm = factory.Build(model);

    await vm.SaveCollectionCommand.ExecuteAsync(null);

    Assert.IsFalse(vm.SaveStatus.HasUnsavedChanges);
    Assert.AreEqual("Name", vm.CollectionName);
  }

  [TestMethod]
  public async Task Save_Override_Cancel_IsDirty()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(true),
      },
      CollectionConfirmers = new()
      {
        ConfirmCollectionSave = (_) => Task.FromResult<string?>("New Name"),
        ConfirmCollectionSaveOverride = (_) => Task.FromResult(ConfirmationResult.Cancel)
      }
    };
    var vm = factory.Build(model);

    await vm.SaveCollectionCommand.ExecuteAsync(null);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Save_Override_NameChanged_IsNotDirty()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new(),
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(true),
        AddResult = (_) => Task.FromResult(true),
      },
      CollectionConfirmers = new()
      {
        ConfirmCollectionSave = (_) => Task.FromResult<string?>("New Name"),
        ConfirmCollectionSaveOverride = (_) => Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build(model);

    await vm.SaveCollectionCommand.ExecuteAsync(null);

    Assert.IsFalse(vm.SaveStatus.HasUnsavedChanges);
    Assert.AreEqual("New Name", vm.CollectionName);
  }

  [TestMethod]
  public async Task Save_Success_NotificationShown()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new(),
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(false),
        AddResult = (_) => Task.FromResult(true),
      },
      CollectionConfirmers = new()
      {
        ConfirmCollectionSave = (_) => Task.FromResult<string?>("New Name"),
      }
    };
    var vm = factory.Build(model);

    await vm.SaveCollectionCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Success, factory.Notifier);
  }

  [TestMethod]
  public async Task Save_Failure_NotificationShown()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(false),
        AddResult = (_) => Task.FromResult(false),
      },
      CollectionConfirmers = new()
      {
        ConfirmCollectionSave = (_) => Task.FromResult<string?>("New Name"),
      }
    };
    var vm = factory.Build(model);

    await vm.SaveCollectionCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Save_Exception_NotificationShown()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      SaveStatus = new() { HasUnsavedChanges = true },
      CollectionConfirmers = new()
      {
        ConfirmCollectionSave = (_) => throw new(),
      }
    };
    var vm = factory.Build(model);

    await vm.SaveCollectionCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}