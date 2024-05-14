using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class CardListViewModelAddCardTests
{
  [TestMethod]
  public void AddCard_CardAdded()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI());

    viewmodel.AddCardCommand.Execute(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    Assert.AreEqual(1, viewmodel.Cards.Count);
  }

  [TestMethod]
  public void AddCard_Undo_CardRemoved()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI());

    viewmodel.AddCardCommand.Execute(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    Assert.AreEqual(1, viewmodel.Cards.Count);

    viewmodel.UndoStack.Undo();

    Assert.AreEqual(0, viewmodel.Cards.Count);
  }

  [TestMethod]
  public void AddCard_Redo_CardAddedAgain()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI());

    viewmodel.AddCardCommand.Execute(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    Assert.AreEqual(1, viewmodel.Cards.Count);

    viewmodel.UndoStack.Undo();

    Assert.AreEqual(0, viewmodel.Cards.Count);

    viewmodel.UndoStack.Redo();

    Assert.AreEqual(1, viewmodel.Cards.Count);
  }
}