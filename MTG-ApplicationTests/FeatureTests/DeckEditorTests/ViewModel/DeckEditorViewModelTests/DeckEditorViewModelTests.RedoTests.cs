using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class RedoTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public void Redo_NoCommands_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM();

      Assert.IsFalse(viewmodel.UndoStack.RedoCommand.CanExecute(null));
    }

    [TestMethod]
    public void Redo_HasCommand_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM();

      viewmodel.DeckCardList.AddCardCommand.Execute(DeckEditorMTGCardMocker.CreateMTGCardModel());
      viewmodel.UndoStack.UndoCommand.Execute(null);

      Assert.IsTrue(viewmodel.UndoStack.RedoCommand.CanExecute(null));
    }

    [TestMethod]
    public void Redo_Execute_ActionInvokedAgain()
    {
      var viewmodel = new Mocker(_dependencies).MockVM();

      viewmodel.DeckCardList.AddCardCommand.Execute(DeckEditorMTGCardMocker.CreateMTGCardModel());

      Assert.AreEqual(1, viewmodel.DeckCardList.Cards.Count);

      viewmodel.UndoStack.UndoCommand.Execute(null);

      Assert.AreEqual(0, viewmodel.DeckCardList.Cards.Count);

      viewmodel.UndoStack.RedoCommand.Execute(null);

      Assert.AreEqual(1, viewmodel.DeckCardList.Cards.Count);
    }
  }
}
