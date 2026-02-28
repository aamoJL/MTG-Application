using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCardList;

[TestClass]
public class Clear
{
  [TestMethod]
  public void Clear_Empty_CanNotExecute()
  {
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = []
    };
    var vm = factory.Build();

    Assert.IsFalse(vm.ClearCommand.CanExecute(null));
  }

  [TestMethod]
  public void Clear_HasCards_CanExecute()
  {
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo(name: "1")),
        new(MTGCardInfoMocker.MockInfo(name: "2")),
        new(MTGCardInfoMocker.MockInfo(name: "3")),
      ]
    };
    var vm = factory.Build();

    Assert.IsTrue(vm.ClearCommand.CanExecute(null));
  }

  [TestMethod]
  public void Clear_Execute()
  {
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo(name: "1")),
        new(MTGCardInfoMocker.MockInfo(name: "2")),
        new(MTGCardInfoMocker.MockInfo(name: "3")),
      ]
    };
    var vm = factory.Build();

    vm.ClearCommand.Execute(null);

    Assert.HasCount(0, vm.CardViewModels);
  }

  [TestMethod]
  public void Clear_Execute_Undo()
  {
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo(name: "1")),
        new(MTGCardInfoMocker.MockInfo(name: "2")),
        new(MTGCardInfoMocker.MockInfo(name: "3")),
      ]
    };
    var vm = factory.Build();

    vm.ClearCommand.Execute(null);
    factory.UndoStack.Undo();

    Assert.HasCount(3, vm.CardViewModels);
  }

  [TestMethod]
  public void Clear_Execute_Redo()
  {
    var factory = new TestDeckCardListViewModelFactory()
    {
      Model = [
        new(MTGCardInfoMocker.MockInfo(name: "1")),
        new(MTGCardInfoMocker.MockInfo(name: "2")),
        new(MTGCardInfoMocker.MockInfo(name: "3")),
      ]
    };
    var vm = factory.Build();

    vm.ClearCommand.Execute(null);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.HasCount(0, vm.CardViewModels);
  }
}
