using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardList;

[TestClass]
public class AddCard
{
  [TestMethod]
  public async Task Add()
  {
    var factory = new TestDeckCardListViewModelFactory();
    var vm = factory.Build();

    var info = MTGCardInfoMocker.MockInfo();

    await vm.AddCardCommand.ExecuteAsync(new DeckEditorMTGCard(info));

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(info, vm.CardViewModels.First().Info);
  }

  [TestMethod]
  public async Task Add_Undo()
  {
    var factory = new TestDeckCardListViewModelFactory();
    var vm = factory.Build();

    var info = MTGCardInfoMocker.MockInfo();

    await vm.AddCardCommand.ExecuteAsync(new DeckEditorMTGCard(info));
    factory.UndoStack.UndoCommand.Execute(null);

    Assert.HasCount(0, vm.CardViewModels);
  }

  [TestMethod]
  public async Task Add_Redo()
  {
    var factory = new TestDeckCardListViewModelFactory();
    var vm = factory.Build();

    var info = MTGCardInfoMocker.MockInfo();

    await vm.AddCardCommand.ExecuteAsync(new DeckEditorMTGCard(info));
    factory.UndoStack.UndoCommand.Execute(null);
    factory.UndoStack.RedoCommand.Execute(null);

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(info, vm.CardViewModels.First().Info);
  }

  [TestMethod]
  public async Task Add_Exception_NotificationShown()
  {
    var factory = new TestDeckCardListViewModelFactory()
    {
      Notifier = new() { ThrowOnError = false }
    };
    var vm = factory.Build();

    await vm.AddCardCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationType.Error, factory.Notifier);
  }

  [TestMethod]
  public async Task IncreaseCount()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info) { Count = 1 }],
      Confirmers = new()
      {
        ConfirmAddSingleConflict = _ => Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    await vm.AddCardCommand.ExecuteAsync(new DeckEditorMTGCard(info) { Count = 2 });

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(3, vm.CardViewModels.First().Count);
  }

  [TestMethod]
  public async Task IncreaseCount_Undo()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info) { Count = 1 }],
      Confirmers = new()
      {
        ConfirmAddSingleConflict = _ => Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    await vm.AddCardCommand.ExecuteAsync(new DeckEditorMTGCard(info) { Count = 2 });
    factory.UndoStack.UndoCommand.Execute(null);

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(1, vm.CardViewModels.First().Count);
  }

  [TestMethod]
  public async Task IncreaseCount_Redo()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info) { Count = 1 }],
      Confirmers = new()
      {
        ConfirmAddSingleConflict = _ => Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    await vm.AddCardCommand.ExecuteAsync(new DeckEditorMTGCard(info) { Count = 2 });
    factory.UndoStack.UndoCommand.Execute(null);
    factory.UndoStack.RedoCommand.Execute(null);

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(3, vm.CardViewModels.First().Count);
  }

  [TestMethod]
  public async Task IncreaseCount_Cancel()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info) { Count = 1 }],
      Confirmers = new()
      {
        ConfirmAddSingleConflict = _ => Task.FromResult(ConfirmationResult.Cancel)
      }
    };
    var vm = factory.Build();

    await vm.AddCardCommand.ExecuteAsync(new DeckEditorMTGCard(info) { Count = 2 });

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(1, vm.CardViewModels.First().Count);
  }
}
