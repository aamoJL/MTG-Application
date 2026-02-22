using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardList;

[TestClass]
public class ImportCards
{
  [TestMethod]
  public async Task Import_New()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1");
    var factory = new TestDeckCardListViewModelFactory()
    {
      Notifier = new(),
      Confirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text")
      },
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new() { Result = TestMTGCardImporter.Success([new(info)]) }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.AreEqual(info, vm.CardViewModels.First().Info);
  }

  [TestMethod]
  public async Task Import_New_Undo()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1");
    var factory = new TestDeckCardListViewModelFactory()
    {
      Notifier = new(),
      Confirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text")
      },
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new() { Result = TestMTGCardImporter.Success([new(info)]) }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();

    Assert.HasCount(0, vm.CardViewModels);
  }

  [TestMethod]
  public async Task Import_New_Redo()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1");
    var factory = new TestDeckCardListViewModelFactory()
    {
      Notifier = new(),
      Confirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text")
      },
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new() { Result = TestMTGCardImporter.Success([new(info)]) }
    };
    var vm = factory.Build();
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.AreEqual(info, vm.CardViewModels.First().Info);
  }

  [TestMethod]
  public async Task Import_WithString_NoConfirmation()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1");
    var factory = new TestDeckCardListViewModelFactory()
    {
      Notifier = new(),
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new() { Result = TestMTGCardImporter.Success([new(info)]) }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync("Import text");

    Assert.AreEqual(info, vm.CardViewModels.First().Info);
  }

  [TestMethod]
  public async Task Import_Exists()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1");
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info)],
      Notifier = new(),
      Confirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text"),
        ConfirmAddMultipleConflict = async _ => await Task.FromResult((ConfirmationResult.Yes, true)),
      },
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new() { Result = TestMTGCardImporter.Success([new(info)]) }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(2, vm.CardViewModels.First().Count);
  }

  [TestMethod]
  public async Task Import_Exists_Undo()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1");
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info)],
      Notifier = new(),
      Confirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text"),
        ConfirmAddMultipleConflict = async _ => await Task.FromResult((ConfirmationResult.Yes, true)),
      },
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new() { Result = TestMTGCardImporter.Success([new(info)]) }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(1, vm.CardViewModels.First().Count);
  }

  [TestMethod]
  public async Task Import_Exists_Redo()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1");
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info)],
      Notifier = new(),
      Confirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text"),
        ConfirmAddMultipleConflict = async _ => await Task.FromResult((ConfirmationResult.Yes, true)),
      },
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new() { Result = TestMTGCardImporter.Success([new(info)]) }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(2, vm.CardViewModels.First().Count);
  }

  [TestMethod]
  public async Task Import_NewAndExisting()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1");
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info)],
      Notifier = new(),
      Confirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text"),
        ConfirmAddMultipleConflict = async _ => await Task.FromResult((ConfirmationResult.Yes, true)),
      },
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([
          new(info),
          new(MTGCardInfoMocker.MockInfo(name: "2")),
        ])
      }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(2, vm.CardViewModels);
    Assert.AreEqual(2, vm.CardViewModels.First().Count);
  }

  [TestMethod]
  public async Task Import_NewAndExisting_Undo()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1");
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info)],
      Notifier = new(),
      Confirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text"),
        ConfirmAddMultipleConflict = async _ => await Task.FromResult((ConfirmationResult.Yes, true)),
      },
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([
          new(info),
          new(MTGCardInfoMocker.MockInfo(name: "2")),
        ])
      }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(1, vm.CardViewModels.First().Count);
  }

  [TestMethod]
  public async Task Import_NewAndExisting_Redo()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1");
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info)],
      Notifier = new(),
      Confirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text"),
        ConfirmAddMultipleConflict = async _ => await Task.FromResult((ConfirmationResult.Yes, true)),
      },
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([
          new(info),
          new(MTGCardInfoMocker.MockInfo(name: "2")),
        ])
      }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.HasCount(2, vm.CardViewModels);
    Assert.AreEqual(2, vm.CardViewModels.First().Count);
  }

  [TestMethod]
  public async Task Import_Exists_Cancel()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1");
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info)],
      Notifier = new(),
      Confirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text"),
        ConfirmAddMultipleConflict = async _ => await Task.FromResult((ConfirmationResult.No, true)),
      },
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new() { Result = TestMTGCardImporter.Success([new(info)]) }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(1, vm.CardViewModels.First().Count);
  }

  [TestMethod]
  public async Task Import_Exception_NotificationShown()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "1");
    var factory = new TestDeckCardListViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false },
      Confirmers = new()
      {
        ConfirmImport = async _ => throw new()
      },
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Error, factory.Notifier);
  }
}
