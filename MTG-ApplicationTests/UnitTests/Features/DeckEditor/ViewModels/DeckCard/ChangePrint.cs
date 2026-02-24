using MTGApplication.General.Models;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCard;

[TestClass]
public class ChangePrint
{
  [TestMethod]
  public async Task Change()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "Changed");
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(oldInfo),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(newInfo)])
      },
      Confirmers = new()
      {
        ConfirmCardPrints = async _ => await Task.FromResult<MTGCard>(new(newInfo))
      }
    };
    var vm = factory.Build();

    await vm.ChangePrintCommand.ExecuteAsync(null);

    Assert.AreEqual(newInfo, vm.Info);
  }

  [TestMethod]
  public async Task Change_Undo()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "Changed");
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(oldInfo),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(newInfo)])
      },
      Confirmers = new()
      {
        ConfirmCardPrints = async _ => await Task.FromResult<MTGCard>(new(newInfo))
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
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "Changed");
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(oldInfo),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(newInfo)])
      },
      Confirmers = new()
      {
        ConfirmCardPrints = async _ => await Task.FromResult<MTGCard>(new(newInfo))
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
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "Changed");
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(oldInfo),
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(newInfo)])
      },
      Confirmers = new()
      {
        ConfirmCardPrints = async _ => await Task.FromResult<MTGCard?>(null)
      }
    };
    var vm = factory.Build();

    await vm.ChangePrintCommand.ExecuteAsync(null);

    Assert.AreEqual(oldInfo, vm.Info);
  }

  [TestMethod]
  public async Task Change_Exception_NotificaionShown()
  {
    var oldInfo = MTGCardInfoMocker.MockInfo(name: "Old");
    var newInfo = MTGCardInfoMocker.MockInfo(name: "Changed");
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(oldInfo),
      Notifier = new() { ThrowOnError = false },
      Importer = new()
      {
        Result = TestMTGCardImporter.Success([new(newInfo)])
      },
      Confirmers = new()
      {
        ConfirmCardPrints = async _ => throw new()
      }
    };
    var vm = factory.Build();

    await vm.ChangePrintCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}