using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
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
      var viewmodel = new CardListViewModel(new TestMTGCardImporter());

      Assert.IsFalse(viewmodel.ClearCommand.CanExecute(null));
    }

    [TestMethod]
    public void Clear_HasCards_CanExecute()
    {
      var viewmodel = new CardListViewModel(new TestMTGCardImporter());
      viewmodel.Cards.Add(DeckEditorMTGCardMocker.CreateMTGCardModel());

      Assert.IsTrue(viewmodel.ClearCommand.CanExecute(null));
    }

    [TestMethod]
    public void Clear_Execute_HasNoCards()
    {
      var viewmodel = new CardListViewModel(new TestMTGCardImporter());
      viewmodel.Cards.Add(DeckEditorMTGCardMocker.CreateMTGCardModel());

      Assert.IsTrue(viewmodel.Cards.Any());

      viewmodel.ClearCommand.Execute(null);

      Assert.IsFalse(viewmodel.Cards.Any());
    }

    [TestMethod]
    public void Clear_Undo_HasCards()
    {
      var viewmodel = new CardListViewModel(new TestMTGCardImporter());
      viewmodel.Cards.Add(DeckEditorMTGCardMocker.CreateMTGCardModel());

      viewmodel.ClearCommand.Execute(null);
      viewmodel.UndoStack.Undo();

      Assert.IsTrue(viewmodel.Cards.Any());
    }

    [TestMethod]
    public void Clear_Redo_HasNoCardsAgain()
    {
      var viewmodel = new CardListViewModel(new TestMTGCardImporter());
      viewmodel.Cards.Add(DeckEditorMTGCardMocker.CreateMTGCardModel());

      viewmodel.ClearCommand.Execute(null);
      viewmodel.UndoStack.Undo();
      viewmodel.UndoStack.Redo();

      Assert.IsFalse(viewmodel.Cards.Any());
    }
  }

}
