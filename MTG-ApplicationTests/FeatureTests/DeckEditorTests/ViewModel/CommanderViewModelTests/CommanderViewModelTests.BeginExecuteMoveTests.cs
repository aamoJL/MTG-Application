using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests.DeckEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;

public partial class CommanderViewModelTests
{
  [TestClass]
  public class BeginExecuteMoveTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public async Task ExecuteMove_CardChanges()
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
}
