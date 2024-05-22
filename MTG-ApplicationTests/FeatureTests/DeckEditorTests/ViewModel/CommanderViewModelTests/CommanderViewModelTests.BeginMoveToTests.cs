using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class BeginMoveToTests
  {
    [TestMethod]
    public async Task BeginMoveTo_CommandAddedToCombinedCommand()
    {
      var card = MTGCardModelMocker.CreateMTGCardModel();
      var viewmodel = new CommanderViewModel(new TestCardAPI());

      await viewmodel.BeginMoveToCommand.ExecuteAsync(card);

      Assert.AreEqual(1, viewmodel.UndoStack.ActiveCombinedCommand.Commands.Count);
    }
  }
}
