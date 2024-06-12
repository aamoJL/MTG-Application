using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests;

public partial class CardListViewModelTests
{
  [TestClass]
  public class RemoveCardTests
  {
    [TestMethod]
    public void RemoveCard_CardRemoved()
    {
      var viewmodel = new CardListViewModel(new TestMTGCardImporter());
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();

      viewmodel.AddCardCommand.Execute(card);
      viewmodel.RemoveCardCommand.Execute(card);

      Assert.AreEqual(0, viewmodel.Cards.Count);
    }

    [TestMethod]
    public void RemoveCard_Undo_CardAdded()
    {
      var viewmodel = new CardListViewModel(new TestMTGCardImporter());
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();

      viewmodel.AddCardCommand.Execute(card);
      viewmodel.RemoveCardCommand.Execute(card);
      viewmodel.UndoStack.Undo();

      Assert.AreEqual(1, viewmodel.Cards.Count);
    }

    [TestMethod]
    public void RemoveCard_Redo_CardRemovedAgain()
    {
      var viewmodel = new CardListViewModel(new TestMTGCardImporter());
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();

      viewmodel.AddCardCommand.Execute(card);
      viewmodel.RemoveCardCommand.Execute(card);
      viewmodel.UndoStack.Undo();
      viewmodel.UndoStack.Redo();

      Assert.AreEqual(0, viewmodel.Cards.Count);
    }
  }
}
