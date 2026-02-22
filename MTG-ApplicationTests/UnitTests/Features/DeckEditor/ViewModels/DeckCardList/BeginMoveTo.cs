using MTGApplication.Features.DeckEditor.Models;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardList;

[TestClass]
public class BeginMoveTo
{
  [TestMethod]
  public async Task BeginMoveTo_CommandAdded()
  {
    var factory = new TestDeckCardListViewModelFactory();
    var vm = factory.Build();

    await vm.BeginMoveToCommand.ExecuteAsync(new(MTGCardInfoMocker.MockInfo()));

    Assert.HasCount(1, factory.UndoStack.ActiveCombinedCommand.Commands);
  }

  [TestMethod]
  public async Task BeginMoveTo_Execute()
  {
    var factory = new TestDeckCardListViewModelFactory();
    var vm = factory.Build();

    var added = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo());
    await vm.BeginMoveToCommand.ExecuteAsync(added);

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();

    CollectionAssert.Contains(factory.Model, added);
  }

  [TestMethod]
  public async Task BeginMoveTo_Execute_Undo()
  {
    var factory = new TestDeckCardListViewModelFactory();
    var vm = factory.Build();

    var added = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo());
    await vm.BeginMoveToCommand.ExecuteAsync(added);

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();

    CollectionAssert.DoesNotContain(factory.Model, added);
  }

  [TestMethod]
  public async Task BeginMoveTo_Execute_Redo()
  {
    var factory = new TestDeckCardListViewModelFactory();
    var vm = factory.Build();

    var added = new DeckEditorMTGCard(MTGCardInfoMocker.MockInfo());
    await vm.BeginMoveToCommand.ExecuteAsync(added);

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    CollectionAssert.Contains(factory.Model, added);
  }

  [TestMethod]
  public async Task BeginMoveTo_ArgumentNull_ExceptionThrown()
  {
    var factory = new TestDeckCardListViewModelFactory();
    var vm = factory.Build();

    await Assert.ThrowsAsync<ArgumentNullException>(() => vm.BeginMoveToCommand.ExecuteAsync(null));
  }
}
