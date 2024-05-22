using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests;

[TestClass]
public class CardListViewModelAddCardTests
{
  [TestMethod]
  public void AddCard_CardAdded()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI());

    viewmodel.AddCardCommand.Execute(MTGCardModelMocker.CreateMTGCardModel());

    Assert.AreEqual(1, viewmodel.Cards.Count);
  }

  [TestMethod]
  public void AddCard_Undo_CardRemoved()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI());

    viewmodel.AddCardCommand.Execute(MTGCardModelMocker.CreateMTGCardModel());

    Assert.AreEqual(1, viewmodel.Cards.Count);

    viewmodel.UndoStack.Undo();

    Assert.AreEqual(0, viewmodel.Cards.Count);
  }

  [TestMethod]
  public void AddCard_Redo_CardAddedAgain()
  {
    var viewmodel = new CardListViewModel(new TestCardAPI());

    viewmodel.AddCardCommand.Execute(MTGCardModelMocker.CreateMTGCardModel());

    Assert.AreEqual(1, viewmodel.Cards.Count);

    viewmodel.UndoStack.Undo();

    Assert.AreEqual(0, viewmodel.Cards.Count);

    viewmodel.UndoStack.Redo();

    Assert.AreEqual(1, viewmodel.Cards.Count);
  }

  [TestMethod]
  public async Task AddCard_Existing_ConflictConfirmationShown()
  {
    var card = MTGCardModelMocker.CreateMTGCardModel();
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