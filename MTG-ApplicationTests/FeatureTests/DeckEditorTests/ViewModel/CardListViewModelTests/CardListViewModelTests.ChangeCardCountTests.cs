using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.Features.DeckEditor.CardList.UseCases.CardListViewModelCommands.ChangeCardCount;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests;

public partial class CardListViewModelTests
{
  [TestClass]
  public class ChangeCardCountTests
  {
    [TestMethod]
    public void ChangeCount_MoreThanOne_CanExecute()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel(new TestMTGCardImporter()) { Cards = [card] };

      Assert.IsTrue(viewmodel.ChangeCardCountCommand.CanExecute(new(card, 3)));
    }

    [TestMethod]
    public void ChangeCount_LessThanOne_CanNotExecute()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel(new TestMTGCardImporter()) { Cards = [card] };

      Assert.IsFalse(viewmodel.ChangeCardCountCommand.CanExecute(new(card, 0)));
    }

    [TestMethod]
    public void ChangeCount_SameCount_CanNotExecute()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel(count: 3);
      var viewmodel = new CardListViewModel(new TestMTGCardImporter()) { Cards = [card] };

      Assert.IsFalse(viewmodel.ChangeCardCountCommand.CanExecute(new(card, card.Count)));
    }

    [TestMethod]
    public void ChangeCount_CountChanged()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel(new TestMTGCardImporter()) { Cards = [card] };

      viewmodel.ChangeCardCountCommand.Execute(new CardCountChangeArgs(card, 3));

      Assert.AreEqual(3, viewmodel.Cards[0].Count);
    }

    [TestMethod]
    public void ChangeCount_Undo_CardHasOriginalCount()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel(new TestMTGCardImporter()) { Cards = [card] };

      viewmodel.ChangeCardCountCommand.Execute(new CardCountChangeArgs(card, 3));
      viewmodel.UndoStack.Undo();

      Assert.AreEqual(1, viewmodel.Cards[0].Count);
    }

    [TestMethod]
    public void ChangeCount_Redo_CountChangedAgain()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel(new TestMTGCardImporter()) { Cards = [card] };

      viewmodel.ChangeCardCountCommand.Execute(new CardCountChangeArgs(card, 3));
      viewmodel.UndoStack.Undo();
      viewmodel.UndoStack.Redo();

      Assert.AreEqual(3, viewmodel.Cards[0].Count);
    }
  }
}
