using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests.DeckEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderCommandsTests
{
  [TestClass]
  public class BeginExecuteMoveTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public async Task ExecuteMove_OnChangesInvoked()
    {
      DeckEditorMTGCard originResult = null;
      DeckEditorMTGCard targetResult = null;

      var undoStack = new ReversibleCommandStack();
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var origin = new CommanderCommands(new Mocker(_dependencies).MockVM(), CommanderCommands.CommanderType.Commander)
      {
        UndoStack = undoStack,
        OnChange = (card) => { originResult = card; }
      };
      var target = new CommanderCommands(new Mocker(_dependencies).MockVM(), CommanderCommands.CommanderType.Partner)
      {
        UndoStack = undoStack,
        OnChange = (card) => { targetResult = card; }
      };

      origin.BeginMoveFromCommand.Execute(card);
      await target.BeginMoveToCommand.ExecuteAsync(card);

      origin.ExecuteMoveCommand.Execute(card);
      target.ExecuteMoveCommand.Execute(card);

      Assert.IsNull(originResult);
      Assert.AreEqual(card.Info.Name, targetResult?.Info.Name);
    }
  }
}
