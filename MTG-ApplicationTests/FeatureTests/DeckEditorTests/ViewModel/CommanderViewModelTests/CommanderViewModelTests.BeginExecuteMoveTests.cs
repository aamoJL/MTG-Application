using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class BeginExecuteMoveTests
  {
    [TestMethod]
    public async Task ExecuteMove_CardMovedBetweenViewModels()
    {
      var undoStack = new ReversibleCommandStack();
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var origin = new CommanderViewModel(new TestCardAPI())
      {
        Card = card,
        UndoStack = undoStack,
      };
      var target = new CommanderViewModel(new TestCardAPI())
      {
        UndoStack = undoStack,
      };

      origin.BeginMoveFromCommand.Execute(card);
      await target.BeginMoveToCommand.ExecuteAsync(card);

      origin.ExecuteMoveCommand.Execute(card);
      target.ExecuteMoveCommand.Execute(card);

      Assert.IsNull(origin.Card);
      Assert.AreEqual(card.Info.Name, target.Card.Info.Name);
    }
  }
}
