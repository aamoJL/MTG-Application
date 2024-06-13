using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class BeginMoveFromTests
  {
    [TestMethod]
    public void BeginMoveFrom_CommandAddedToCombinedCommand()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new CommanderViewModel(new TestMTGCardImporter(), () => null);

      viewmodel.BeginMoveFromCommand.Execute(card);

      Assert.AreEqual(1, viewmodel.UndoStack.ActiveCombinedCommand.Commands.Count);
    }
  }
}
