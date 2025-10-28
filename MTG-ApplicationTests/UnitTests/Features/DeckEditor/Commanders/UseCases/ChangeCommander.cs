using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.UnitTests.Features.DeckEditor.Editor.ViewModels;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.Commanders.UseCases;

[TestClass]
public class ChangeCommander : DeckEditorViewModelTestBase
{
  [TestMethod]
  public async Task Change_ToCard_CardChanged()
  {
    var viewmodel = new CommanderViewModel(_dependencies.Importer);
    var card = DeckEditorMTGCardMocker.CreateMTGCardModel();

    await viewmodel.ChangeCommanderCommand.ExecuteAsync(card);

    Assert.AreEqual(card, viewmodel.Card);

    viewmodel.UndoStack.Undo();

    Assert.IsNull(viewmodel.Card);

    viewmodel.UndoStack.Redo();

    Assert.AreEqual(card, viewmodel.Card);
  }

  [TestMethod]
  public async Task Change_ToNull_CardRemoved()
  {
    var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
    var viewmodel = new CommanderViewModel(_dependencies.Importer)
    {
      Card = card
    };

    await viewmodel.ChangeCommanderCommand.ExecuteAsync(null);

    Assert.IsNull(viewmodel.Card);

    viewmodel.UndoStack.Undo();

    Assert.AreEqual(card, viewmodel.Card);

    viewmodel.UndoStack.Redo();

    Assert.IsNull(viewmodel.Card);
  }
}
