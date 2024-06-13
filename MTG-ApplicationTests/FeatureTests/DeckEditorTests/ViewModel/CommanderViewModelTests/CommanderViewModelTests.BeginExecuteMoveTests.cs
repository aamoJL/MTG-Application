using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.Features.DeckEditor.Editor.Models;
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
    public async Task ExecuteMove_OnChangesInvoked()
    {
      DeckEditorMTGCard? originResult = null;
      DeckEditorMTGCard? targetResult = null;

      var undoStack = new ReversibleCommandStack();
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var origin = new CommanderViewModel(new TestMTGCardImporter(), () => card)
      {
        UndoStack = undoStack,
        OnChange = (card) => { originResult = card; }
      };
      var target = new CommanderViewModel(new TestMTGCardImporter(), () => null)
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
