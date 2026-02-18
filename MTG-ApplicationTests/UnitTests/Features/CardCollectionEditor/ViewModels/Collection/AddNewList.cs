using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.Collection;

[TestClass]
public class AddNewList
{
  [TestMethod]
  public async Task Add_Cancel_Return()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      CollectionConfirmers = new()
      {
        ConfirmAddNewList = (_) => Task.FromResult<(string, string)?>(null)
      },
    };
    var vm = factory.Build(model);

    await vm.AddNewListCommand.ExecuteAsync(null);

    Assert.HasCount(0, model.CollectionLists);
  }

  [TestMethod]
  public async Task Add_NoName_NotificationShown()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      CollectionConfirmers = new()
      {
        ConfirmAddNewList = (_) => Task.FromResult<(string, string)?>((string.Empty, "query"))
      },
    };
    var vm = factory.Build(model);

    await vm.AddNewListCommand.ExecuteAsync(null);

    Assert.HasCount(0, model.CollectionLists);
    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Add_NoQuery_NotificationShown()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      CollectionConfirmers = new()
      {
        ConfirmAddNewList = (_) => Task.FromResult<(string, string)?>(("Name", string.Empty))
      },
    };
    var vm = factory.Build(model);

    await vm.AddNewListCommand.ExecuteAsync(null);

    Assert.HasCount(0, model.CollectionLists);
    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Add_Exists_NotificationShown()
  {
    var model = new MTGCardCollection()
    {
      CollectionLists = [new() { Name = "Name" }]
    };
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      CollectionConfirmers = new()
      {
        ConfirmAddNewList = (_) => Task.FromResult<(string, string)?>(("Name", "query"))
      },
    };
    var vm = factory.Build(model);

    await vm.AddNewListCommand.ExecuteAsync(null);

    Assert.HasCount(1, model.CollectionLists);
    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Add_Success_ListAdded()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success()
      },
      CollectionConfirmers = new()
      {
        ConfirmAddNewList = (_) => Task.FromResult<(string, string)?>(("Name", "query"))
      },
    };
    var vm = factory.Build(model);

    await vm.AddNewListCommand.ExecuteAsync(null);

    Assert.HasCount(1, model.CollectionLists);
    Assert.AreEqual("Name", model.CollectionLists.First().Name);
    Assert.AreEqual("query", model.CollectionLists.First().SearchQuery);
  }

  [TestMethod]
  public async Task Add_Success_IsDirty()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success()
      },
      CollectionConfirmers = new()
      {
        ConfirmAddNewList = (_) => Task.FromResult<(string, string)?>(("Name", "query"))
      },
    };
    var vm = factory.Build(model);

    await vm.AddNewListCommand.ExecuteAsync(null);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Add_Success_ListViewModelAdded()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success()
      },
      CollectionConfirmers = new()
      {
        ConfirmAddNewList = (_) => Task.FromResult<(string, string)?>(("Name", "query"))
      },
    };
    var vm = factory.Build(model);

    await vm.AddNewListCommand.ExecuteAsync(null);

    Assert.HasCount(1, vm.CollectionListViewModels);
  }

  [TestMethod]
  public async Task Add_Success_ListSelected()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success()
      },
      CollectionConfirmers = new()
      {
        ConfirmAddNewList = (_) => Task.FromResult<(string, string)?>(("Name", "query"))
      },
    };
    var vm = factory.Build(model);

    await vm.AddNewListCommand.ExecuteAsync(null);

    Assert.AreEqual("Name", vm.ListViewModel?.Name);
  }

  [TestMethod]
  public async Task Add_Success_NotificationShown()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success()
      },
      CollectionConfirmers = new()
      {
        ConfirmAddNewList = (_) => Task.FromResult<(string, string)?>(("Name", "query"))
      },
    };
    var vm = factory.Build(model);

    await vm.AddNewListCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Success, factory.Notifier);
  }

  [TestMethod]
  public async Task Add_Exception_NotificationShown()
  {
    var model = new MTGCardCollection();
    var factory = new TestCollectionViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      CollectionConfirmers = new()
      {
        ConfirmAddNewList = (_) => throw new()
      },
    };
    var vm = factory.Build(model);

    await vm.AddNewListCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}