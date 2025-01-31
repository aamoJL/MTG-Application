using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardListViewModelCommands;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class ChangeCardCountCommandTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public void ChangeCount_CountChanged()
    {
      var oldValue = 1;
      var newValue = 2;

      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(count: oldValue);

      var viewmodel = new Mocker(_dependencies).MockVM();
      var args = new CardCountChangeArgs(card, newValue);

      viewmodel.ChangeCardCountCommand.Execute(args);

      Assert.AreEqual(newValue, card.Count);

      viewmodel.UndoStack.Undo();

      Assert.AreEqual(oldValue, card.Count);

      viewmodel.UndoStack.Redo();

      Assert.AreEqual(newValue, card.Count);
    }
  }
}

