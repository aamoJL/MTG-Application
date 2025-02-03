using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using static MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests.DeckEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class RemoveCommanderCommandTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public void Remove_CardRemoved()
    {
      var card = _savedDeck.Commander;
      var viewmodel = new CommanderViewModel(_dependencies.Importer)
      {
        Card = card,
      };

      viewmodel.RemoveCommanderCommand.Execute(null);

      Assert.IsNull(viewmodel.Card);

      viewmodel.UndoStack.Undo();

      Assert.AreEqual(card, viewmodel.Card);

      viewmodel.UndoStack.Redo();

      Assert.IsNull(viewmodel.Card);
    }
  }
}
