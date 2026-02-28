using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.NotificationService;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;

[TestClass]
public class AddCard
{
  [TestMethod]
  public async Task Add()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", []),
    };
    var vm = factory.Build();

    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Name")) { Count = 1, Group = string.Empty };
    await vm.AddCardCommand.ExecuteAsync(card);

    Assert.HasCount(1, vm.CardViewModels);
    Assert.HasCount(1, factory.Model.Cards);
  }

  [TestMethod]
  public async Task Add_Undo()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", []),
    };
    var vm = factory.Build();

    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Name")) { Count = 1, Group = string.Empty };
    await vm.AddCardCommand.ExecuteAsync(card);
    factory.UndoStack.Undo();

    Assert.HasCount(0, vm.CardViewModels);
    Assert.HasCount(0, factory.Model.Cards);
  }

  [TestMethod]
  public async Task Add_Redo()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", []),
    };
    var vm = factory.Build();

    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Name")) { Count = 1, Group = string.Empty };
    await vm.AddCardCommand.ExecuteAsync(card);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.HasCount(1, factory.Model.Cards);
  }

  [TestMethod]
  public async Task IncreaseCount()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [
        new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Name")) { Count = 1, Group = "Group" },
      ]),
      ListConfirmers = new()
      {
        ConfirmAddSingleConflict = async _ => await Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Name")) { Count = 2, Group = string.Empty };
    await vm.AddCardCommand.ExecuteAsync(card);

    Assert.AreEqual(3, vm.Size);
    Assert.HasCount(1, vm.CardViewModels);
    Assert.HasCount(1, factory.Model.Cards);
  }

  [TestMethod]
  public async Task IncreaseCount_Undo()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [
        new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Name")) { Count = 1, Group = "Group" },
      ]),
      ListConfirmers = new()
      {
        ConfirmAddSingleConflict = async _ => await Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Name")) { Count = 2, Group = string.Empty };
    await vm.AddCardCommand.ExecuteAsync(card);
    factory.UndoStack.Undo();

    Assert.AreEqual(1, vm.Size);
    Assert.HasCount(1, vm.CardViewModels);
    Assert.HasCount(1, factory.Model.Cards);
  }

  [TestMethod]
  public async Task IncreaseCount_Redo()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", [
        new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Name")) { Count = 1, Group = "Group" },
      ]),
      ListConfirmers = new()
      {
        ConfirmAddSingleConflict = async _ => await Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Name")) { Count = 2, Group = string.Empty };
    await vm.AddCardCommand.ExecuteAsync(card);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.AreEqual(3, vm.Size);
    Assert.HasCount(1, vm.CardViewModels);
    Assert.HasCount(1, factory.Model.Cards);
  }

  [TestMethod]
  public async Task Add_Exception_NotificationShown()
  {
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", []),
      Notifier = new() { ThrowOnError = false }
    };
    var vm = factory.Build();

    await vm.AddCardCommand.ExecuteAsync(null);

    NotificationAssert.NotificationSent(NotificationService.NotificationType.Error, factory.Notifier);
  }
}
