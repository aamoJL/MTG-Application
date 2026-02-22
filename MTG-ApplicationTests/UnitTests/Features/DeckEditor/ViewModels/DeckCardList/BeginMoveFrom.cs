using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardList;

[TestClass]
public class BeginMoveFrom
{
  [TestMethod]
  public void BeginMoveFrom_CommandAdded()
  {
    var factory = new TestDeckCardListViewModelFactory();
    var vm = factory.Build();

    vm.BeginMoveFromCommand.Execute(new(MTGCardInfoMocker.MockInfo()));

    Assert.HasCount(1, factory.UndoStack.ActiveCombinedCommand.Commands);
  }

  [TestMethod]
  public void BeginMoveFrom_Execute()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info)]
    };
    var vm = factory.Build();

    vm.BeginMoveFromCommand.Execute(new(info));

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();

    Assert.HasCount(0, vm.CardViewModels);
  }

  [TestMethod]
  public void BeginMoveFrom_Execute_Undo()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info)]
    };
    var vm = factory.Build();

    vm.BeginMoveFromCommand.Execute(new(info));

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();

    Assert.HasCount(1, vm.CardViewModels);
  }

  [TestMethod]
  public void BeginMoveFrom_Execute_Redo()
  {
    var info = MTGCardInfoMocker.MockInfo();
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [new(info)]
    };
    var vm = factory.Build();

    vm.BeginMoveFromCommand.Execute(new(info));

    factory.UndoStack.PushAndExecuteActiveCombinedCommand();
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.HasCount(0, vm.CardViewModels);
  }

  [TestMethod]
  public void BeginMoveFrom_ArgumentNull_ExceptionThrown()
  {
    var factory = new TestDeckCardListViewModelFactory();
    var vm = factory.Build();

    Assert.Throws<ArgumentNullException>(() => vm.BeginMoveFromCommand.Execute(null));
  }
}
