using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.ViewModels;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class UndoTests : DeckEditorViewModelTestBase
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

      Assert.HasCount(1, viewmodel.DeckCardList.Cards);

      viewmodel.UndoStack.UndoCommand.Execute(null);

      Assert.IsEmpty(viewmodel.DeckCardList.Cards);
    }
  }
}

