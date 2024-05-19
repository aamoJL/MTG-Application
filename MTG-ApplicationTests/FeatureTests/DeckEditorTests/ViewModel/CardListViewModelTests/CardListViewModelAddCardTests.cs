using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;

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

  [TestMethod]
  public async Task AddCard_Existing_ConflictConfirmationShown()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var viewmodel = new CardListViewModel(new TestCardAPI())
    {
      Cards = [card],
      Confirmers = new()
      {
        AddSingleConflictConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
      }
    };

    await ConfirmationAssert.ConfirmationShown(() => viewmodel.AddCardCommand.ExecuteAsync(card));
  }
}