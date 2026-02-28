using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Mocker;
using System.Collections.ObjectModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardGroup.CardGroup;

[TestClass]
public class BeginMoveTo
{
  [TestMethod]
  public async Task BeginMoveTo_CommandAdded()
  {
    var factory = new TestDeckCardGroupViewModelFactory();
    var vm = factory.Build();

    await vm.BeginMoveToCommand.ExecuteAsync(new(MTGCardInfoMocker.MockInfo()));

    Assert.HasCount(1, factory.UndoStack.ActiveCombinedCommand.Commands);
  }

  [TestMethod]
  public async Task BeginMoveTo_NotInSource()
  {
    var factory = new TestDeckCardGroupViewModelFactory();
    var vm = factory.Build();

    var added = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Card"));
    await vm.BeginMoveToCommand.ExecuteAsync(added);

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();

    Assert.AreEqual("Card", factory.Model.Cards.First().Info.Name);
  }

  [TestMethod]
  public async Task BeginMoveTo_NotInSource_Undo()
  {
    var factory = new TestDeckCardGroupViewModelFactory();
    var vm = factory.Build();

    var added = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Card"));
    await vm.BeginMoveToCommand.ExecuteAsync(added);

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();

    Assert.HasCount(0, factory.Model.Cards);
  }

  [TestMethod]
  public async Task BeginMoveTo_NotInSource_Redo()
  {
    var factory = new TestDeckCardGroupViewModelFactory();
    var vm = factory.Build();

    var added = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Card"));
    await vm.BeginMoveToCommand.ExecuteAsync(added);

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.AreEqual("Card", factory.Model.Cards.First().Info.Name);
  }

  [TestMethod]
  public async Task BeginMoveTo_InSource()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Card")) { Group = string.Empty };
    var source = new ObservableCollection<DeckEditorMTGCard>() { card };
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", source),
      ListConfirmers = new()
      {
        ConfirmAddSingleConflict = async _ => await Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    Assert.HasCount(0, vm.CardViewModels);
    Assert.HasCount(1, source);

    await vm.BeginMoveToCommand.ExecuteAsync(card);

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.HasCount(1, source);
  }

  [TestMethod]
  public async Task BeginMoveTo_InSource_Undo()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Card")) { Group = string.Empty };
    var source = new ObservableCollection<DeckEditorMTGCard>() { card };
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", source),
      ListConfirmers = new()
      {
        ConfirmAddSingleConflict = async _ => await Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    await vm.BeginMoveToCommand.ExecuteAsync(card);

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();

    Assert.HasCount(0, vm.CardViewModels);
    Assert.HasCount(1, source);
  }

  [TestMethod]
  public async Task BeginMoveTo_InSource_Redo()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Card")) { Group = string.Empty };
    var source = new ObservableCollection<DeckEditorMTGCard>() { card };
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", source),
      ListConfirmers = new()
      {
        ConfirmAddSingleConflict = async _ => await Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    await vm.BeginMoveToCommand.ExecuteAsync(card);

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.HasCount(1, source);
  }

  [TestMethod]
  public async Task BeginMoveTo_InGroup()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Card")) { Group = "Group" };
    var source = new ObservableCollection<DeckEditorMTGCard>() { card };
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", source),
      ListConfirmers = new()
      {
        ConfirmAddSingleConflict = async _ => await Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.HasCount(1, source);

    await vm.BeginMoveToCommand.ExecuteAsync(new(card.Info) { Group = string.Empty, Count = 3 });

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(4, vm.Size);
    Assert.HasCount(1, source);
  }

  [TestMethod]
  public async Task BeginMoveTo_InGroup_Undo()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Card")) { Group = "Group" };
    var source = new ObservableCollection<DeckEditorMTGCard>() { card };
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", source),
      ListConfirmers = new()
      {
        ConfirmAddSingleConflict = async _ => await Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.HasCount(1, source);

    await vm.BeginMoveToCommand.ExecuteAsync(new(card.Info) { Group = string.Empty, Count = 3 });

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(1, vm.Size);
    Assert.HasCount(1, source);
  }

  [TestMethod]
  public async Task BeginMoveTo_InGroup_Redo()
  {
    var card = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo(name: "Card")) { Group = "Group" };
    var source = new ObservableCollection<DeckEditorMTGCard>() { card };
    var factory = new TestDeckCardGroupViewModelFactory()
    {
      Model = new("Group", source),
      ListConfirmers = new()
      {
        ConfirmAddSingleConflict = async _ => await Task.FromResult(ConfirmationResult.Yes)
      }
    };
    var vm = factory.Build();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.HasCount(1, source);

    await vm.BeginMoveToCommand.ExecuteAsync(new(card.Info) { Group = string.Empty, Count = 3 });

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.HasCount(1, vm.CardViewModels);
    Assert.AreEqual(4, vm.Size);
    Assert.HasCount(1, source);
  }

  [TestMethod]
  public async Task BeginMoveTo_ArgumentNull_ExceptionThrown()
  {
    var factory = new TestDeckCardGroupViewModelFactory();
    var vm = factory.Build();

    await Assert.ThrowsAsync<ArgumentNullException>(() => vm.BeginMoveToCommand.ExecuteAsync(null));
  }
}
