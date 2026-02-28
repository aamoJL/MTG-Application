using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.DeckCard;

[TestClass]
public class ChangeTag
{
  [TestMethod]
  public void Change()
  {
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(MTGCardInfoMocker.MockInfo(name: "Card"))
    };
    var vm = factory.Build();

    vm.ChangeTagCommand.Execute(MTGApplication.General.Models.CardTag.Add);

    Assert.AreEqual(MTGApplication.General.Models.CardTag.Add, vm.CardTag);
  }

  [TestMethod]
  public void Change_Undo()
  {
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(MTGCardInfoMocker.MockInfo(name: "Card"))
    };
    var vm = factory.Build();

    vm.ChangeTagCommand.Execute(MTGApplication.General.Models.CardTag.Add);
    factory.UndoStack.Undo();

    Assert.IsNull(vm.CardTag);
  }

  [TestMethod]
  public void Change_Redo()
  {
    var factory = new TestDeckCardViewModelFactory()
    {
      Model = new(MTGCardInfoMocker.MockInfo(name: "Card"))
    };
    var vm = factory.Build();

    vm.ChangeTagCommand.Execute(MTGApplication.General.Models.CardTag.Add);
    factory.UndoStack.Undo();
    factory.UndoStack.Redo();

    Assert.AreEqual(MTGApplication.General.Models.CardTag.Add, vm.CardTag);
  }
}
