using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Commanders.ViewModels;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests.DeckEditorViewModelTests;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CommanderViewModelTests;
public partial class CommanderViewModelTests
{
  [TestClass]
  public class ChangeCommanderCommandTests : DeckEditorViewModelTestsBase
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
}
