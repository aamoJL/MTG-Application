using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests;

public partial class CardListViewModelTests
{
  [TestClass]
  public class ChangeCardCountTests
  {
    [TestMethod]
    public void ChangeCount_MoreThanOne_CanExecute()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel(new TestCardAPI()) { Cards = [card] };

      Assert.IsTrue(viewmodel.ChangeCardCountCommand.CanExecute(new(card, 3)));
    }

    [TestMethod]
    public void ChangeCount_LessThanOne_CanNotExecute()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel(new TestCardAPI()) { Cards = [card] };

      Assert.IsFalse(viewmodel.ChangeCardCountCommand.CanExecute(new(card, 0)));
    }

    [TestMethod]
    public void ChangeCount_SameCount_CanNotExecute()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel(count: 3);
      var viewmodel = new CardListViewModel(new TestCardAPI()) { Cards = [card] };

      Assert.IsFalse(viewmodel.ChangeCardCountCommand.CanExecute(new(card, card.Count)));
    }

    [TestMethod]
    public void ChangeCount_CountChanged()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel(new TestCardAPI()) { Cards = [card] };

      viewmodel.ChangeCardCountCommand.Execute(new CardListViewModel.CardCountChangeArgs(card, 3));

      Assert.AreEqual(3, viewmodel.Cards[0].Count);
    }

    [TestMethod]
    public void ChangeCount_Undo_CardHasOriginalCount()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel(new TestCardAPI()) { Cards = [card] };

      viewmodel.ChangeCardCountCommand.Execute(new CardListViewModel.CardCountChangeArgs(card, 3));
      viewmodel.UndoStack.Undo();

      Assert.AreEqual(1, viewmodel.Cards[0].Count);
    }

    [TestMethod]
    public void ChangeCount_Redo_CountChangedAgain()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel(new TestCardAPI()) { Cards = [card] };

      viewmodel.ChangeCardCountCommand.Execute(new CardListViewModel.CardCountChangeArgs(card, 3));
      viewmodel.UndoStack.Undo();
      viewmodel.UndoStack.Redo();

      Assert.AreEqual(3, viewmodel.Cards[0].Count);
    }
  }
}
