using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.CollectionList;

[TestClass]
public class EditList
{
  [TestMethod]
  public async Task Edit_NameAndQueryInConfirmer()
  {
    var name = string.Empty;
    var query = string.Empty;
    var model = new MTGCardCollectionList()
    {
      Name = "Name",
      SearchQuery = "Query"
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      CollectionListConfirmers = new()
      {
        ConfirmEditList = async (data) =>
        {
          name = data.Data.name;
          query = data.Data.query;
          return await Task.FromResult<(string, string)?>(null);
        }
      },
    };
    var vm = factory.Build(model);

    await vm.EditListCommand.ExecuteAsync(null);

    Assert.AreEqual("Name", name);
    Assert.AreEqual("Query", query);
  }

  [TestMethod]
  public async Task Edit_Cancel_Return()
  {
    var model = new MTGCardCollectionList()
    {
      Name = "Old"
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      CollectionListConfirmers = new()
      {
        ConfirmEditList = (_) => Task.FromResult<(string, string)?>(null)
      },
    };
    var vm = factory.Build(model);

    await vm.EditListCommand.ExecuteAsync(null);

    Assert.AreEqual("Old", model.Name);
  }

  [TestMethod]
  public async Task Edit_NoName_NotificationShown()
  {
    var model = new MTGCardCollectionList()
    {
      Name = "Old"
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      CollectionListConfirmers = new()
      {
        ConfirmEditList = (_) => Task.FromResult<(string, string)?>((string.Empty, "query"))
      },
    };
    var vm = factory.Build(model);

    await vm.EditListCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Edit_NoQuery_NotificationShown()
  {
    var model = new MTGCardCollectionList()
    {
      Name = "Old",
      SearchQuery = "Query"
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      CollectionListConfirmers = new()
      {
        ConfirmEditList = (_) => Task.FromResult<(string, string)?>(("New", string.Empty))
      },
    };
    var vm = factory.Build(model);

    await vm.EditListCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Edit_InvalidName_NotificationShown()
  {
    var model = new MTGCardCollectionList()
    {
      Name = "Old"
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      CollectionListConfirmers = new()
      {
        ConfirmEditList = (_) => Task.FromResult<(string, string)?>(("New", "query"))
      },
      NameValidator = false
    };
    var vm = factory.Build(model);

    await vm.EditListCommand.ExecuteAsync(null);

    Assert.AreEqual("Old", model.Name);
    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Edit_NewName_NameChanged()
  {
    var model = new MTGCardCollectionList()
    {
      SearchQuery = "query"
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      CollectionListConfirmers = new()
      {
        ConfirmEditList = (_) => Task.FromResult<(string, string)?>(("Name", "query"))
      },
      NameValidator = true
    };
    var vm = factory.Build(model);

    await vm.EditListCommand.ExecuteAsync(null);

    Assert.AreEqual("Name", vm.Name);
  }

  [TestMethod]
  public async Task Edit_NewQuery_QueryChanged()
  {
    var model = new MTGCardCollectionList()
    {
      Name = "Name"
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = CardImportResult.Empty()
      },
      CollectionListConfirmers = new()
      {
        ConfirmEditList = (_) => Task.FromResult<(string, string)?>(("Name", "query"))
      },
    };
    var vm = factory.Build(model);

    await vm.EditListCommand.ExecuteAsync(null);

    Assert.AreEqual("query", vm.Query);
  }

  [TestMethod]
  public async Task Edit_NewQuery_Conflict_Cancel_Cancelled()
  {
    var cards = MTGCardMocker.Mock(5).ToArray();
    var model = new MTGCardCollectionList()
    {
      Name = "Name",
      Cards = [.. cards]
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([.. cards.Take(3).Select(x => new CardImportResult.Card(x.Info))])
      },
      CollectionListConfirmers = new()
      {
        ConfirmEditList = (_) => Task.FromResult<(string, string)?>(("Name", "query")),
        ConfirmEditQueryConflict = (_) => Task.FromResult(ConfirmationResult.Cancel)
      },
    };
    var vm = factory.Build(model);

    await vm.EditListCommand.ExecuteAsync(null);

    Assert.AreEqual("Name", vm.Name);
    Assert.AreEqual(string.Empty, vm.Query);
  }

  [TestMethod]
  public async Task Edit_NewQuery_Conflict_Accept_CardsRemoved()
  {
    var cards = MTGCardMocker.Mock(5).ToArray();
    var model = new MTGCardCollectionList()
    {
      Name = "Name",
      Cards = [.. cards]
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([.. cards.Take(3).Select(x => new CardImportResult.Card(x.Info))])
      },
      CollectionListConfirmers = new()
      {
        ConfirmEditList = (_) => Task.FromResult<(string, string)?>(("Name", "query")),
        ConfirmEditQueryConflict = (_) => Task.FromResult(ConfirmationResult.Yes)
      },
    };
    var vm = factory.Build(model);

    await vm.EditListCommand.ExecuteAsync(null);

    Assert.HasCount(3, vm.Cards);
    Assert.AreEqual("Name", vm.Name);
    Assert.AreEqual("query", vm.Query);
  }

  public async Task Edit_Success_IsDirty()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Importer = new()
      {
        Result = CardImportResult.Empty()
      },
      CollectionListConfirmers = new()
      {
        ConfirmEditList = (_) => Task.FromResult<(string, string)?>(("Name", "query"))
      },
      NameValidator = true
    };
    var vm = factory.Build(model);

    await vm.EditListCommand.ExecuteAsync(null);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Edit_Success_NotificationShown()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = CardImportResult.Empty()
      },
      CollectionListConfirmers = new()
      {
        ConfirmEditList = (_) => Task.FromResult<(string, string)?>(("Name", "query"))
      },
      NameValidator = true
    };
    var vm = factory.Build(model);

    await vm.EditListCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Success, factory.Notifier);
  }

  [TestMethod]
  public async Task Edit_Exception_NotificationShown()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      CollectionListConfirmers = new()
      {
        ConfirmEditList = (_) => throw new()
      },
    };
    var vm = factory.Build(model);

    await vm.EditListCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}