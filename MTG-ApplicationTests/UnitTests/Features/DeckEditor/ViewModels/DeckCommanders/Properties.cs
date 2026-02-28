using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Models;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using MTGApplicationTests.TestUtility.ViewModel;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCommanders;

[TestClass]
public class Properties
{
  [TestMethod]
  public void Init_Info()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "Card");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new()
      {
        Commander = new(info)
      }
    };
    var vm = factory.Build();

    Assert.AreEqual(info, vm.Info);
  }

  [TestMethod]
  public void Change_ModelInfo_InfoChanged()
  {
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new()
      {
        Commander = new(MTGCardInfoMocker.MockInfo(name: "Card"))
      }
    };
    var vm = factory.Build();

    var changed = MTGCardInfoMocker.MockInfo(name: "Changed");
    vm.AssertPropertyChanged(nameof(vm.Info),
      () => factory.Model.Commander.Info = changed);

    Assert.AreEqual(changed, vm.Info);
  }

  [TestMethod]
  public void Change_Model_InfoChanged()
  {
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new()
      {
        Commander = new(MTGCardInfoMocker.MockInfo(name: "Card"))
      }
    };
    var vm = factory.Build();

    var changed = MTGCardInfoMocker.MockInfo(name: "Changed");
    vm.AssertPropertyChanged(nameof(vm.Info),
      () => factory.Model.Commander = new(changed));

    Assert.AreEqual(changed, vm.Info);
  }

  [TestMethod]
  public void Init_Name()
  {
    var info = MTGCardInfoMocker.MockInfo(name: "Card");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new()
      {
        Commander = new(info)
      }
    };
    var vm = factory.Build();

    Assert.AreEqual("Card", vm.Info?.Name);
  }

  [TestMethod]
  public void Change_Model_NameChanged()
  {
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new()
      {
        Commander = new(MTGCardInfoMocker.MockInfo(name: "Card"))
      }
    };
    var vm = factory.Build();

    var changed = MTGCardInfoMocker.MockInfo(name: "Changed");
    vm.AssertPropertyChanged(nameof(vm.Name),
      () => factory.Model.Commander = new(changed));

    Assert.AreEqual("Changed", vm.Name);
  }
}

[TestClass]
public class ChangeCard
{
  [TestMethod]
  public async Task Change()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) }
    };
    var vm = factory.Build();

    var newInfo = MTGCardInfoMocker.MockInfo(name: "New", typeLine: "Legendary");
    await vm.ChangeCardCommand.ExecuteAsync(new(newInfo));

    Assert.AreEqual(newInfo, vm.Info);
  }

  [TestMethod]
  public async Task Change_Undo()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) }
    };
    var vm = factory.Build();

    var newInfo = MTGCardInfoMocker.MockInfo(name: "New", typeLine: "Legendary");
    await vm.ChangeCardCommand.ExecuteAsync(new(newInfo));
    factory.UndoStack.Undo();

    Assert.AreEqual(oldInfo, vm.Info);
  }

  [TestMethod]
  public async Task Change_Redo()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) }
    };
    var vm = factory.Build();

    var newInfo = MTGCardInfoMocker.MockInfo(name: "New", typeLine: "Legendary");
    await vm.ChangeCardCommand.ExecuteAsync(new(newInfo));
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.AreEqual(newInfo, vm.Info);
  }

  [TestMethod]
  public async Task Change_NotLegendary_NotificationShown()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) },
      Dependencies = new()
      {
        Notifier = new() { ThrowOnError = false },
      }
    };
    var vm = factory.Build();

    var newInfo = MTGCardInfoMocker.MockInfo(name: "New");
    await vm.ChangeCardCommand.ExecuteAsync(new(newInfo));

    NotificationAssert.NotificationSent(NotificationType.Error, factory.Dependencies.Notifier);
  }
}

[TestClass]
public class DeleteCard
{
  [TestMethod]
  public async Task Delete_CommanderNull_CanNotExecute()
  {
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = null }
    };
    var vm = factory.Build();

    Assert.IsFalse(vm.DeleteCardCommand.CanExecute(null));
  }

  [TestMethod]
  public async Task Delete_CommanderNotNull_CanExecute()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) }
    };
    var vm = factory.Build();

    Assert.IsTrue(vm.DeleteCardCommand.CanExecute(null));
  }

  [TestMethod]
  public async Task Delete()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) }
    };
    var vm = factory.Build();

    vm.DeleteCardCommand.Execute(null);

    Assert.IsNull(vm.Info);
  }

  [TestMethod]
  public async Task Delete_Undo()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) }
    };
    var vm = factory.Build();

    vm.DeleteCardCommand.Execute(null);
    factory.UndoStack.Undo();

    Assert.AreEqual(oldInfo, vm.Info);
  }

  [TestMethod]
  public async Task Delete_Redo()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) }
    };
    var vm = factory.Build();

    vm.DeleteCardCommand.Execute(null);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.IsNull(vm.Info);
  }
}

[TestClass]
public class ImportCard
{
  [TestMethod]
  public async Task Import()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "New", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) },
      Dependencies = new()
      {
        Notifier = new(),
        EdhrecImporter = new() { ParseResult = string.Empty },
        Importer = new() { Result = TestMTGCardImporter.Success([new(newInfo)]) }
      }
    };
    var vm = factory.Build();

    await vm.ImportCardCommand.ExecuteAsync("Import text");

    Assert.AreEqual(newInfo, vm.Info);
  }

  [TestMethod]
  public async Task Import_Undo()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "New", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) },
      Dependencies = new()
      {
        Notifier = new(),
        EdhrecImporter = new() { ParseResult = string.Empty },
        Importer = new() { Result = TestMTGCardImporter.Success([new(newInfo)]) }
      }
    };
    var vm = factory.Build();

    await vm.ImportCardCommand.ExecuteAsync("Import text");
    factory.UndoStack.Undo();

    Assert.AreEqual(oldInfo, vm.Info);
  }
  [TestMethod]
  public async Task Import_Redo()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "New", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) },
      Dependencies = new()
      {
        Notifier = new(),
        EdhrecImporter = new() { ParseResult = string.Empty },
        Importer = new() { Result = TestMTGCardImporter.Success([new(newInfo)]) }
      }
    };
    var vm = factory.Build();

    await vm.ImportCardCommand.ExecuteAsync("Import text");
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.AreEqual(newInfo, vm.Info);
  }

  [TestMethod]
  public async Task Import_ArgumentNull_NotificationShown()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) },
      Dependencies = new()
      {
        Notifier = new() { ThrowOnError = false },
      },
    };
    var vm = factory.Build();

    await vm.ImportCardCommand.ExecuteAsync("Import text");

    NotificationAssert.NotificationSent(NotificationType.Error, factory.Dependencies.Notifier);
  }

  [TestMethod]
  public async Task Import_NotLegendary_NotificationShown()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "New", typeLine: string.Empty);
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) },
      Dependencies = new()
      {
        Notifier = new() { ThrowOnError = false },
        EdhrecImporter = new() { ParseResult = string.Empty },
        Importer = new() { Result = TestMTGCardImporter.Success([new(newInfo)]) }
      }
    };
    var vm = factory.Build();

    await vm.ImportCardCommand.ExecuteAsync("Import text");

    NotificationAssert.NotificationSent(NotificationType.Error, factory.Dependencies.Notifier);
  }

  [TestMethod]
  public async Task Import_MultipleCards_NotificationShown()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "New", typeLine: string.Empty);
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) },
      Dependencies = new()
      {
        Notifier = new() { ThrowOnError = false },
        EdhrecImporter = new() { ParseResult = string.Empty },
        Importer = new() { Result = TestMTGCardImporter.Success([new(newInfo), new(newInfo)]) }
      }
    };
    var vm = factory.Build();

    await vm.ImportCardCommand.ExecuteAsync("Import text");

    NotificationAssert.NotificationSent(NotificationType.Error, factory.Dependencies.Notifier);
  }

  [TestMethod]
  public async Task Import_NoCards_NotificationShown()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) },
      Dependencies = new()
      {
        Notifier = new() { ThrowOnError = false },
        EdhrecImporter = new() { ParseResult = string.Empty },
        Importer = new() { Result = TestMTGCardImporter.Failure() }
      }
    };
    var vm = factory.Build();

    await vm.ImportCardCommand.ExecuteAsync("Import text");

    NotificationAssert.NotificationSent(NotificationType.Error, factory.Dependencies.Notifier);
  }
}

[TestClass]
public class BeginMoveFrom
{
  [TestMethod]
  public void BeginMoveFrom_CommandAdded()
  {
    var commander = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = commander }
    };
    var vm = factory.Build();

    vm.BeginMoveFromCommand.Execute(commander);

    Assert.HasCount(1, factory.UndoStack.ActiveCombinedCommand.Commands);
  }

  [TestMethod]
  public void BeginMoveFrom_Execute()
  {
    var commander = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = commander }
    };
    var vm = factory.Build();

    vm.BeginMoveFromCommand.Execute(commander);

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();

    Assert.IsNull(factory.Model.Commander);
  }

  [TestMethod]
  public void BeginMoveFrom_Execute_Undo()
  {
    var commander = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = commander }
    };
    var vm = factory.Build();

    vm.BeginMoveFromCommand.Execute(commander);

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();

    Assert.AreEqual(commander, factory.Model.Commander);
  }

  [TestMethod]
  public void BeginMoveFrom_Execute_Redo()
  {
    var commander = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = commander }
    };
    var vm = factory.Build();

    vm.BeginMoveFromCommand.Execute(commander);

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.IsNull(factory.Model.Commander);
  }

  [TestMethod]
  public void BeginMoveFrom_ArgumentNull_ExceptionThown()
  {
    var commander = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = commander }
    };
    var vm = factory.Build();

    Assert.Throws<ArgumentNullException>(() => vm.BeginMoveFromCommand.Execute(null));
  }

  [TestMethod]
  public void BeginMoveFrom_ArgumentNotCommander_ExceptionThown()
  {
    var commander = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = commander }
    };
    var vm = factory.Build();

    Assert.Throws<InvalidOperationException>(() => vm.BeginMoveFromCommand.Execute(new(MTGCardInfoMocker.MockInfo())));
  }
}

[TestClass]
public class BeginMoveTo
{
  [TestMethod]
  public async Task BeginMoveTo_CommandAdded()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) }
    };
    var vm = factory.Build();

    await vm.BeginMoveToCommand.ExecuteAsync(new(MTGCardInfoMocker.MockInfo(name: "New")));

    Assert.HasCount(1, factory.UndoStack.ActiveCombinedCommand.Commands);
  }

  [TestMethod]
  public async Task BeginMoveTo_Execute()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) }
    };
    var vm = factory.Build();

    var newInfo = MTGCardInfoMocker.MockInfo(name: "New");
    await vm.BeginMoveToCommand.ExecuteAsync(new(newInfo));

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();

    Assert.AreEqual(newInfo, factory.Model.Commander.Info);
  }

  [TestMethod]
  public async Task BeginMoveTo_Execute_Undo()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) }
    };
    var vm = factory.Build();

    var newInfo = MTGCardInfoMocker.MockInfo(name: "New");
    await vm.BeginMoveToCommand.ExecuteAsync(new(newInfo));

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();

    Assert.AreEqual(oldInfo, factory.Model.Commander.Info);
  }

  [TestMethod]
  public async Task BeginMoveTo_Execute_Redo()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) }
    };
    var vm = factory.Build();

    var newInfo = MTGCardInfoMocker.MockInfo(name: "New");
    await vm.BeginMoveToCommand.ExecuteAsync(new(newInfo));

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.AreEqual(newInfo, factory.Model.Commander.Info);
  }

  [TestMethod]
  public async Task BeginMoveTo_ArgumentNull_ExceptionThrown()
  {
    var commander = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo());
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = commander }
    };
    var vm = factory.Build();

    await Assert.ThrowsAsync<ArgumentNullException>(() => vm.BeginMoveToCommand.ExecuteAsync(null));
  }
}

[TestClass]
public class ExecuteMove
{
  [TestMethod]
  public async Task Execute()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) }
    };
    var vm = factory.Build();

    Assert.HasCount(0, factory.UndoStack.ActiveCombinedCommand.Commands);
    Assert.AreEqual(oldInfo, factory.Model.Commander.Info);

    var newInfo = MTGCardInfoMocker.MockInfo(name: "New");
    await vm.BeginMoveToCommand.ExecuteAsync(new(newInfo));

    Assert.HasCount(1, factory.UndoStack.ActiveCombinedCommand.Commands);
    Assert.AreEqual(oldInfo, factory.Model.Commander.Info);

    vm.ExecuteMoveCommand.Execute(null);

    Assert.HasCount(0, factory.UndoStack.ActiveCombinedCommand.Commands);
    Assert.AreEqual(newInfo, factory.Model.Commander.Info);
  }
}

[TestClass]
public class ChangePrint
{
  [TestMethod]
  public async Task Change_CommanderNull_CanNotExecute()
  {
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = null }
    };
    var vm = factory.Build();

    Assert.IsFalse(vm.ChangePrintCommand.CanExecute(null));
  }

  [TestMethod]
  public async Task Change_CommanderNotNull_CanExecute()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) }
    };
    var vm = factory.Build();

    Assert.IsTrue(vm.ChangePrintCommand.CanExecute(null));
  }

  [TestMethod]
  public async Task Change()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "New", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) },
      Dependencies = new()
      {
        Importer = new() { Result = TestMTGCardImporter.Success([new(newInfo)]) },
        CardConfirmers = new()
        {
          ConfirmCardPrints = async _ => await Task.FromResult(new MTGCard(newInfo))
        }
      }
    };
    var vm = factory.Build();

    await vm.ChangePrintCommand.ExecuteAsync(null);

    Assert.AreEqual(newInfo, vm.Info);
  }

  [TestMethod]
  public async Task Change_Undo()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "New", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) },
      Dependencies = new()
      {
        Importer = new() { Result = TestMTGCardImporter.Success([new(newInfo)]) },
        CardConfirmers = new()
        {
          ConfirmCardPrints = async _ => await Task.FromResult(new MTGCard(newInfo))
        }
      }
    };
    var vm = factory.Build();

    await vm.ChangePrintCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();

    Assert.AreEqual(oldInfo, vm.Info);
  }

  [TestMethod]
  public async Task Change_Redo()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "New", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) },
      Dependencies = new()
      {
        Importer = new() { Result = TestMTGCardImporter.Success([new(newInfo)]) },
        CardConfirmers = new()
        {
          ConfirmCardPrints = async _ => await Task.FromResult(new MTGCard(newInfo))
        }
      }
    };
    var vm = factory.Build();

    await vm.ChangePrintCommand.ExecuteAsync(null);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.AreEqual(newInfo, vm.Info);
  }

  [TestMethod]
  public async Task Change_Cancel()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old", typeLine: "Legendary");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "New", typeLine: "Legendary");
    var factory = new TestCommanderViewModelFactory()
    {
      Model = new() { Commander = new(oldInfo) },
      Dependencies = new()
      {
        Importer = new() { Result = TestMTGCardImporter.Success([new(newInfo)]) },
        CardConfirmers = new()
        {
          ConfirmCardPrints = async _ => await Task.FromResult<MTGCard?>(null)
        }
      }
    };
    var vm = factory.Build();

    await vm.ChangePrintCommand.ExecuteAsync(null);

    Assert.AreEqual(oldInfo, vm.Info);
  }
}