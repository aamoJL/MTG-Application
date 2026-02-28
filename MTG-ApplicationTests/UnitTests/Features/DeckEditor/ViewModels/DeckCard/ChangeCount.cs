using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCard;

[TestClass]
public class ChangeCount
{
  [TestMethod]
  public void Change()
  {
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(MTGCardInfoMocker.MockInfo(name: "Card"))
    };
    var vm = factory.Build();

    vm.ChangeCountCommand.Execute(4);

    Assert.AreEqual(4, vm.Count);
  }

  [TestMethod]
  public void Change_Undo()
  {
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(MTGCardInfoMocker.MockInfo(name: "Card"))
    };
    var vm = factory.Build();

    vm.ChangeCountCommand.Execute(4);
    factory.UndoStack.Undo();

    Assert.AreEqual(1, vm.Count);
  }

  [TestMethod]
  public void Change_Redo()
  {
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(MTGCardInfoMocker.MockInfo(name: "Card"))
    };
    var vm = factory.Build();

    vm.ChangeCountCommand.Execute(4);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.AreEqual(4, vm.Count);
  }
}
