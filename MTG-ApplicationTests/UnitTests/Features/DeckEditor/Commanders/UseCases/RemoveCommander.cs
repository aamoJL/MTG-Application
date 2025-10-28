using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.ViewModels;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.Commanders.UseCases;

[TestClass]
public class RemoveCommander : DeckEditorViewModelTestBase
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
