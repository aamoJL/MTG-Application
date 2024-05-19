using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class CardListViewModelMoveTests
{
  [TestMethod("Command should be added to the combined command when starting the move to action")]
  public void Move_BeginTo_CommandAdded()
  {
    var target = new CardListViewModel(new TestCardAPI());
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    target.BeginMoveToCommand.Execute(card);

    Assert.AreEqual(1, target.UndoStack.ActiveCombinedCommand.Commands.Count);
  }

  [TestMethod("Command should be added to the combined command when starting the move from action")]
  public void Move_From_CommandAdded()
  {
    var origin = new CardListViewModel(new TestCardAPI());

    origin.BeginMoveFromCommand.Execute(Mocker.MTGCardModelMocker.CreateMTGCardModel());

    Assert.AreEqual(1, origin.UndoStack.ActiveCombinedCommand.Commands.Count);
  }

  [TestMethod("Card should change list when executing move command")]
  public void Move_Execute_CardChangesList()
  {
    var target = new CardListViewModel(new TestCardAPI());
    var origin = new CardListViewModel(new TestCardAPI());
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    origin.AddCardCommand.Execute(card);

    Assert.AreEqual(1, origin.Cards.Count);

    origin.BeginMoveFromCommand.Execute(card);
    target.BeginMoveToCommand.Execute(card);
    target.ExecuteMoveCommand.Execute(card);
    origin.ExecuteMoveCommand.Execute(card);

    Assert.AreEqual(0, origin.Cards.Count);
    Assert.AreEqual(1, target.Cards.Count);
  }

  [TestMethod("Card should be in the origin list when undoing the move command")]
  public void Move_Undo_CardInOriginalList()
  {
    var undoStack = new ReversibleCommandStack();
    var target = new CardListViewModel(new TestCardAPI()) { UndoStack = undoStack };
    var origin = new CardListViewModel(new TestCardAPI()) { UndoStack = undoStack };
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    origin.AddCardCommand.Execute(card);
    origin.BeginMoveFromCommand.Execute(card);
    target.BeginMoveToCommand.Execute(card);
    target.ExecuteMoveCommand.Execute(card);

    undoStack.Undo();

    Assert.AreEqual(1, origin.Cards.Count);
    Assert.AreEqual(0, target.Cards.Count);
  }

  [TestMethod("Card should be in the target list when redoing the move command")]
  public void Move_Redo_CardChangedListAgain()
  {
    var undoStack = new ReversibleCommandStack();
    var target = new CardListViewModel(new TestCardAPI()) { UndoStack = undoStack };
    var origin = new CardListViewModel(new TestCardAPI()) { UndoStack = undoStack };
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();

    origin.AddCardCommand.Execute(card);
    origin.BeginMoveFromCommand.Execute(card);
    target.BeginMoveToCommand.Execute(card);
    target.ExecuteMoveCommand.Execute(card);

    undoStack.Undo();
    undoStack.Redo();

    Assert.AreEqual(0, origin.Cards.Count);
    Assert.AreEqual(1, target.Cards.Count);
  }

  [TestMethod]
  public async Task MoveTo_AlreadyExists_ConflictConfirmationShown()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var target = new CardListViewModel(new TestCardAPI())
    {
      Cards = [card],
      Confirmers = new()
      {
        AddSingleConflictConfirmer = new TestExceptionConfirmer<ConfirmationResult>()
      }
    };

    await ConfirmationAssert.ConfirmationShown(() => target.BeginMoveToCommand.ExecuteAsync(card));
  }

  [TestMethod("Card should change list when executing move command")]
  public async Task Move_CancelConflict_DoesNotExecute_CardInOriginList()
  {
    var undoStack = new ReversibleCommandStack();
    var originCard = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var targetCard = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var target = new CardListViewModel(new TestCardAPI())
    {
      Cards = [targetCard],
      UndoStack = undoStack,
      Confirmers = new()
      {
        AddSingleConflictConfirmer = new()
        {
          OnConfirm = async (arg) => await Task.FromResult(ConfirmationResult.No)
        }
      }
    };
    var origin = new CardListViewModel(new TestCardAPI())
    {
      UndoStack = undoStack,
      Cards = [originCard],
    };

    origin.BeginMoveFromCommand.Execute(originCard);
    await target.BeginMoveToCommand.ExecuteAsync(originCard);

    Assert.IsFalse(undoStack.ActiveCombinedCommand.CanExecute());

    target.ExecuteMoveCommand.Execute(originCard);
    origin.ExecuteMoveCommand.Execute(originCard);

    Assert.IsFalse(undoStack.CanUndo);
    Assert.AreEqual(originCard, origin.Cards.First());
    Assert.AreEqual(1, target.Cards.Sum(x => x.Count));
  }
}