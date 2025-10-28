using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.ViewModels;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.Commanders.UseCases;

public class MoveCard
{
  [TestClass]
  public class ExecuteMove : DeckEditorViewModelTestBase
  {
    [TestMethod]
    public async Task ExecuteMove_CommandsExecuted()
    {
      var undoStack = new ReversibleCommandStack();
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var origin = new CommanderViewModel(_dependencies.Importer)
      {
        Card = card,
        UndoStack = undoStack
      };
      var target = new CommanderViewModel(_dependencies.Importer)
      {
        Card = null,
        UndoStack = undoStack
      };

      origin.BeginMoveFromCommand.Execute(card);
      await target.BeginMoveToCommand.ExecuteAsync(card);

      origin.ExecuteMoveCommand.Execute(card);
      target.ExecuteMoveCommand.Execute(card);

      Assert.IsNull(origin.Card);
      Assert.AreEqual(card, target.Card);

      target.UndoStack.Undo();

      Assert.IsNull(target.Card);
      Assert.AreEqual(card, origin.Card);

      target.UndoStack.Redo();

      Assert.IsNull(origin.Card);
      Assert.AreEqual(card, target.Card);
    }
  }

  [TestClass]
  public class BeginMoveFrom : DeckEditorViewModelTestBase
  {
    [TestMethod]
    public void BeginMoveFrom_CommandAddedToCombinedCommand()
    {
      var viewmodel = new CommanderViewModel(_dependencies.Importer);
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();

      viewmodel.BeginMoveFromCommand.Execute(card);

      Assert.HasCount(1, viewmodel.UndoStack.ActiveCombinedCommand.Commands);
    }
  }

  [TestClass]
  public class BeginMoveTo : DeckEditorViewModelTestBase
  {
    [TestMethod]
    public async Task BeginMoveTo_CommandAddedToCombinedCommand()
    {
      var viewmodel = new CommanderViewModel(_dependencies.Importer);
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();

      await viewmodel.BeginMoveToCommand.ExecuteAsync(card);

      Assert.HasCount(1, viewmodel.UndoStack.ActiveCombinedCommand.Commands);
    }
  }
}
