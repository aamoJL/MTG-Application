using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.ViewModels;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class RedoTests : DeckEditorViewModelTestBase
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

      Assert.HasCount(1, viewmodel.DeckCardList.Cards);

      viewmodel.UndoStack.UndoCommand.Execute(null);

      Assert.IsEmpty(viewmodel.DeckCardList.Cards);

      viewmodel.UndoStack.RedoCommand.Execute(null);

      Assert.HasCount(1, viewmodel.DeckCardList.Cards);
    }
  }
}
