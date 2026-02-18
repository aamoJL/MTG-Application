using MTGApplication.Features.CardCollectionEditor.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.CollectionList;

[TestClass]
public class ImportCards
{
  [TestMethod]
  public async Task Import_Cancel_Return()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo())])
      },
      CollectionListConfirmers = new()
      {
        ConfirmCardImport = _ => Task.FromResult<string?>(null)
      }
    };
    var vm = factory.Build(model);

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(0, vm.Cards);
  }

  [TestMethod]
  public async Task Import_Empty_Return()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo())])
      },
      CollectionListConfirmers = new()
      {
        ConfirmCardImport = _ => Task.FromResult<string?>(string.Empty)
      }
    };
    var vm = factory.Build(model);

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(0, vm.Cards);
  }

  [TestMethod]
  public async Task Import_Success_CardsAdded()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo())])
      },
      CollectionListConfirmers = new()
      {
        ConfirmCardImport = _ => Task.FromResult<string?>("query")
      }
    };
    var vm = factory.Build(model);

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(1, vm.Cards);
  }

  public async Task Import_Success_IsDirty()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo())])
      },
      CollectionListConfirmers = new()
      {
        ConfirmCardImport = _ => Task.FromResult<string?>("query")
      }
    };
    var vm = factory.Build(model);

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.IsTrue(vm.SaveStatus.HasUnsavedChanges);
  }

  [TestMethod]
  public async Task Import_PartialSuccess_CardsAdded()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Partial([new(MTGCardInfoMocker.MockInfo())])
      },
      CollectionListConfirmers = new()
      {
        ConfirmCardImport = _ => Task.FromResult<string?>("query")
      }
    };
    var vm = factory.Build(model);

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(1, vm.Cards);
  }

  [TestMethod]
  public async Task Import_Success_NewCardsAdded()
  {
    var model = new MTGCardCollectionList()
    {
      Cards = [.. MTGCardMocker.Mock(5)]
    };
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([
          new(MTGCardInfoMocker.MockInfo()),
          .. model.Cards.Take(2).Select(x => new CardImportResult.Card(x.Info))
          ])
      },
      CollectionListConfirmers = new()
      {
        ConfirmCardImport = _ => Task.FromResult<string?>("query")
      }
    };
    var vm = factory.Build(model);

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(6, vm.Cards);
  }

  [TestMethod]
  public async Task Import_Failure_NotificationShown()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      Importer = new()
      {
        Result = TestMTGCardImporter.Failure()
      },
      CollectionListConfirmers = new()
      {
        ConfirmCardImport = _ => Task.FromResult<string?>("query")
      }
    };
    var vm = factory.Build(model);

    await vm.ImportCardsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task Import_Success_NotificationShown()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo())])
      },
      CollectionListConfirmers = new()
      {
        ConfirmCardImport = _ => Task.FromResult<string?>("query")
      }
    };
    var vm = factory.Build(model);

    await vm.ImportCardsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Success, factory.Notifier);
  }

  [TestMethod]
  public async Task Import_PartialSuccess_NotificationShown()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new(),
      Importer = new()
      {
        Result = TestMTGCardImporter.Partial([new(MTGCardInfoMocker.MockInfo())])
      },
      CollectionListConfirmers = new()
      {
        ConfirmCardImport = _ => Task.FromResult<string?>("query")
      }
    };
    var vm = factory.Build(model);

    await vm.ImportCardsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Warning, factory.Notifier);
  }

  [TestMethod]
  public async Task Import_Exception_NotificationShown()
  {
    var model = new MTGCardCollectionList();
    var factory = new TestCollectionListViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      CollectionListConfirmers = new()
      {
        ConfirmCardImport = _ => throw new()
      }
    };
    var vm = factory.Build(model);

    await vm.ImportCardsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}