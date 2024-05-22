using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

[TestClass]
public class DeckEditorViewModelRedoTests : DeckEditorViewModelTestsBase
{
  [TestMethod]
  public void Redo_NoCommands_CanNotExecute()
  {
    var viewmodel = MockVM();

    Assert.IsFalse(viewmodel.RedoCommand.CanExecute(null));
  }

  [TestMethod]
  public void Redo_HasCommand_CanExecute()
  {
    var viewmodel = MockVM();

    viewmodel.DeckCardList.AddCardCommand.Execute(MTGCardModelMocker.CreateMTGCardModel());
    viewmodel.UndoCommand.Execute(null);

    Assert.IsTrue(viewmodel.RedoCommand.CanExecute(null));
  }

  [TestMethod]
  public void Redo_Execute_ActionInvokedAgain()
  {
    var viewmodel = MockVM();

    viewmodel.DeckCardList.AddCardCommand.Execute(MTGCardModelMocker.CreateMTGCardModel());

    Assert.AreEqual(1, viewmodel.DeckCardList.Cards.Count);

    viewmodel.UndoCommand.Execute(null);

    Assert.AreEqual(0, viewmodel.DeckCardList.Cards.Count);

    viewmodel.RedoCommand.Execute(null);

    Assert.AreEqual(1, viewmodel.DeckCardList.Cards.Count);
  }
}
