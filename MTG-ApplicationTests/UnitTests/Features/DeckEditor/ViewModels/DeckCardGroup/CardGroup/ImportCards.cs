using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;

[TestClass]
public class ImportCards
{
  [TestMethod]
  public async Task Import_New()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Notifier = new(),
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo(name: "Card"))])
      },
      ListConfirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text")
      }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(1, vm.CardViewModels);
  }

  [TestMethod]
  public async Task Import_New_Undo()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Notifier = new(),
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo(name: "Card"))])
      },
      ListConfirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text")
      }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();

    Assert.HasCount(0, vm.CardViewModels);
  }

  [TestMethod]
  public async Task Import_New_Redo()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Notifier = new(),
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo(name: "Card"))])
      },
      ListConfirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text")
      }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.HasCount(1, vm.CardViewModels);
  }

  [TestMethod]
  public async Task Import_Exists()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [new(MTGCardInfoMocker.MockInfo(name: "Card")) { Group = "Group" }]),
      Notifier = new(),
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo(name: "Card"))])
      },
      ListConfirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text"),
        ConfirmAddMultipleConflict = async _ => await Task.FromResult((ConfirmationResult.Yes, true))
      }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(2, vm.Size);
  }

  [TestMethod]
  public async Task Import_Exists_Undo()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [new(MTGCardInfoMocker.MockInfo(name: "Card")) { Group = "Group" }]),
      Notifier = new(),
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo(name: "Card"))])
      },
      ListConfirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text"),
        ConfirmAddMultipleConflict = async _ => await Task.FromResult((ConfirmationResult.Yes, true))
      }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(1, vm.Size);
  }

  [TestMethod]
  public async Task Import_Exists_Redo()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [new(MTGCardInfoMocker.MockInfo(name: "Card")) { Group = "Group" }]),
      Notifier = new(),
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(MTGCardInfoMocker.MockInfo(name: "Card"))])
      },
      ListConfirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text"),
        ConfirmAddMultipleConflict = async _ => await Task.FromResult((ConfirmationResult.Yes, true))
      }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(2, vm.Size);
  }

  [TestMethod]
  public async Task Import_NewAndExisting()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [new(MTGCardInfoMocker.MockInfo(name: "Card")) { Group = "Group" }]),
      Notifier = new(),
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([
          new(MTGCardInfoMocker.MockInfo(name: "Card")),
          new(MTGCardInfoMocker.MockInfo(name: "Card 2")),
        ])
      },
      ListConfirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text"),
        ConfirmAddMultipleConflict = async _ => await Task.FromResult((ConfirmationResult.Yes, true))
      }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(2, vm.CardViewModels);
    Assert.AreEqual(3, vm.Size);
  }

  [TestMethod]
  public async Task Import_NewAndExisting_Undo()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [new(MTGCardInfoMocker.MockInfo(name: "Card")) { Group = "Group" }]),
      Notifier = new(),
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([
          new(MTGCardInfoMocker.MockInfo(name: "Card")),
          new(MTGCardInfoMocker.MockInfo(name: "Card 2")),
        ])
      },
      ListConfirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text"),
        ConfirmAddMultipleConflict = async _ => await Task.FromResult((ConfirmationResult.Yes, true))
      }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(1, vm.Size);
  }

  [TestMethod]
  public async Task Import_NewAndExisting_Redo()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [new(MTGCardInfoMocker.MockInfo(name: "Card")) { Group = "Group" }]),
      Notifier = new(),
      EdhrecImporter = new() { ParseResult = string.Empty },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([
          new(MTGCardInfoMocker.MockInfo(name: "Card")),
          new(MTGCardInfoMocker.MockInfo(name: "Card 2")),
        ])
      },
      ListConfirmers = new()
      {
        ConfirmImport = async _ => await Task.FromResult("Import text"),
        ConfirmAddMultipleConflict = async _ => await Task.FromResult((ConfirmationResult.Yes, true))
      }
    };
    var vm = factory.Build();

    await vm.ImportCardsCommand.ExecuteAsync(null);

    Assert.HasCount(2, vm.CardViewModels);
    Assert.AreEqual(3, vm.Size);
  }
}
