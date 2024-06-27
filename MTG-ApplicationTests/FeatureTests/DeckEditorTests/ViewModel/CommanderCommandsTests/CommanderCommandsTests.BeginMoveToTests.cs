using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests.DeckEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderCommandsTests
{
  [TestClass]
  public class BeginMoveToTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public async Task BeginMoveTo_CommandAddedToCombinedCommand()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new CommanderCommands(new Mocker(_dependencies).MockVM(), CommanderCommands.CommanderType.Commander);

      await viewmodel.BeginMoveToCommand.ExecuteAsync(card);

      Assert.AreEqual(1, viewmodel.UndoStack.ActiveCombinedCommand.Commands.Count);
    }
  }
}
