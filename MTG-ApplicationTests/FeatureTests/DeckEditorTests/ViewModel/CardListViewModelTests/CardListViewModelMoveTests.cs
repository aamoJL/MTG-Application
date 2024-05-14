using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

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
}