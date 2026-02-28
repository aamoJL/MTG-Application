using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.Deck;

[TestClass]
public class SaveDeck
{
  [TestMethod]
  public async Task Save_NameInConfirmer()
  {
    var name = string.Empty;
    var factory = new TestDeckViewModelFactory()
    {
      Model = new() { Name = "Name" },
      SaveStatus = new() { HasUnsavedChanges = true },
      Confirmers = new()
      {
        ConfirmDeckSave = async (data) => { name = data.Data; return await Task.FromResult<string?>(null); },
      }
    };
    var vm = factory.Build();

    await vm.SaveDeckCommand.ExecuteAsync(null);

    Assert.AreEqual("Name", name);
  }

  [TestMethod]
  public async Task Save_Cancel_IsDirty()
  {
    var factory = new TestDeckViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      Confirmers = new()
      {
        ConfirmDeckSave = (_) => Task.FromResult<string?>(null),
      }
    };
    var vm = factory.Build();

    await vm.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Save_InvalidName_IsDirty()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      SaveStatus = new() { HasUnsavedChanges = true },
      Confirmers = new()
      {
        ConfirmDeckSave = (_) => Task.FromResult<string?>(string.Empty),
      }
    };
    var vm = factory.Build();

    await vm.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Save_NewName_NameChanged_IsNotDirty()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Notifier = new(),
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(false),
        AddResult = (_) => Task.FromResult(true),
      },
      Confirmers = new()
      {
        ConfirmDeckSave = (_) => Task.FromResult<string?>("New Name"),
      }
    };
    var vm = factory.Build();

    await vm.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(vm.SaveStatus.HasUnsavedChanges);
    Assert.AreEqual("New Name", vm.DeckName);
  }

  [TestMethod]
  public async Task Save_SameName_IsNotDirty()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Model = new() { Name = "Name" },
      Notifier = new(),
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(false),
        AddResult = (_) => Task.FromResult(true),
      },
      Confirmers = new()
      {
        ConfirmDeckSave = (_) => Task.FromResult<string?>("Name"),
      }
    };
    var vm = factory.Build();

    await vm.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(vm.SaveStatus.HasUnsavedChanges);
    Assert.AreEqual("Name", vm.DeckName);
  }

  [TestMethod]
  public async Task Save_Override_Cancel_IsDirty()
  {
    var factory = new TestDeckViewModelFactory()
    {
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(true),
      },
      Confirmers = new()
      {
        ConfirmDeckSave = (_) => Task.FromResult<string?>("New Name"),
        ConfirmDeckSaveOverride = (_) => Task.FromResult(ConfirmationResult.Cancel)
      }
    };
    var vm = factory.Build();

    await vm.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Save_Override_NameChanged_IsNotDirty()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Notifier = new(),
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(true),
        AddResult = (_) => Task.FromResult(true),
      },
      Confirmers = new()
      {
        ConfirmDeckSave = (_) => Task.FromResult<string?>("New Name"),
        ConfirmDeckSaveOverride = (_) => Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    await vm.SaveDeckCommand.ExecuteAsync(null);

    Assert.IsFalse(vm.SaveStatus.HasUnsavedChanges);
    Assert.AreEqual("New Name", vm.DeckName);
  }

  [TestMethod]
  public async Task Save_Success_NotificationShown()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Notifier = new(),
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(false),
        AddResult = (_) => Task.FromResult(true),
      },
      Confirmers = new()
      {
        ConfirmDeckSave = (_) => Task.FromResult<string?>("New Name"),
      }
    };
    var vm = factory.Build();

    await vm.SaveDeckCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Success, factory.Notifier);
  }

  [TestMethod]
  public async Task Save_Failure_NotificationShown()
  {
    var factory = new TestDeckViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      SaveStatus = new() { HasUnsavedChanges = true },
      Repository = new()
      {
        ExistsResult = (_) => Task.FromResult(false),
        AddResult = (_) => Task.FromResult(false),
      },
      Confirmers = new()
      {
        ConfirmDeckSave = (_) => Task.FromResult<string?>("New Name"),
      }
    };
    var vm = factory.Build();

    await vm.SaveDeckCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
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
        ConfirmDeckSave = (_) => throw new(),
      }
    };
    var vm = factory.Build();

    await vm.SaveDeckCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}
