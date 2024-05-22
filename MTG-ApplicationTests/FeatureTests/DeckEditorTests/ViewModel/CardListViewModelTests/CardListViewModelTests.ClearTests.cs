using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests;

public partial class CardListViewModelTests
{
  [TestClass]
  public class ClearTests
  {
    [TestMethod]
    public void Clear_Empty_CanNotExecute()
    {
      var viewmodel = new CardListViewModel(new TestCardAPI());

      Assert.IsFalse(viewmodel.ClearCommand.CanExecute(null));
    }

    [TestMethod]
    public void Clear_HasCards_CanExecute()
    {
      var viewmodel = new CardListViewModel(new TestCardAPI());
      viewmodel.Cards.Add(MTGCardModelMocker.CreateMTGCardModel());

      Assert.IsTrue(viewmodel.ClearCommand.CanExecute(null));
    }

    [TestMethod]
    public void Clear_Execute_HasNoCards()
    {
      var viewmodel = new CardListViewModel(new TestCardAPI());
      viewmodel.Cards.Add(MTGCardModelMocker.CreateMTGCardModel());

      Assert.IsTrue(viewmodel.Cards.Any());

      viewmodel.ClearCommand.Execute(null);

      Assert.IsFalse(viewmodel.Cards.Any());
    }

    [TestMethod]
    public void Clear_Undo_HasCards()
    {
      var viewmodel = new CardListViewModel(new TestCardAPI());
      viewmodel.Cards.Add(MTGCardModelMocker.CreateMTGCardModel());

      viewmodel.ClearCommand.Execute(null);
      viewmodel.UndoStack.Undo();

      Assert.IsTrue(viewmodel.Cards.Any());
    }

    [TestMethod]
    public void Clear_Redo_HasNoCardsAgain()
    {
      var viewmodel = new CardListViewModel(new TestCardAPI());
      viewmodel.Cards.Add(MTGCardModelMocker.CreateMTGCardModel());

      viewmodel.ClearCommand.Execute(null);
      viewmodel.UndoStack.Undo();
      viewmodel.UndoStack.Redo();

      Assert.IsFalse(viewmodel.Cards.Any());
    }
  }

}
