using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class UndoTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public void Undo_NoCommands_CanNotExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM();

      Assert.IsFalse(viewmodel.UndoStack.UndoCommand.CanExecute(null));
    }

    [TestMethod]
    public void Undo_HasCommand_CanExecute()
    {
      var viewmodel = new Mocker(_dependencies).MockVM();

      viewmodel.DeckCardList.AddCardCommand.Execute(DeckEditorMTGCardMocker.CreateMTGCardModel());

      Assert.IsTrue(viewmodel.UndoStack.UndoCommand.CanExecute(null));
    }

    [TestMethod]
    public void Undo_Execute_ReverseActionInvoked()
    {
      var viewmodel = new Mocker(_dependencies).MockVM();

      viewmodel.DeckCardList.AddCardCommand.Execute(DeckEditorMTGCardMocker.CreateMTGCardModel());

      Assert.AreEqual(1, viewmodel.DeckCardList.Cards.Count);

      viewmodel.UndoStack.UndoCommand.Execute(null);

      Assert.AreEqual(0, viewmodel.DeckCardList.Cards.Count);
    }
  }
}

