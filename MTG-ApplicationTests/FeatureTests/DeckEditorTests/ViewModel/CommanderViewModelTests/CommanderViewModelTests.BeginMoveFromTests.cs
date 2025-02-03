using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests.DeckEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class BeginMoveFromTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public void BeginMoveFrom_CommandAddedToCombinedCommand()
    {
      var viewmodel = new CommanderViewModel(_dependencies.Importer);
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();

      viewmodel.BeginMoveFromCommand.Execute(card);

      Assert.AreEqual(1, viewmodel.UndoStack.ActiveCombinedCommand.Commands.Count);
    }
  }
}
