using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.ConfirmationService;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.CardListViewModelTests;

public partial class CardListViewModelTests
{
  [TestClass]
  public class AddCardTests
  {
    [TestMethod]
    public void AddCard_CardAdded()
    {
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter());

      viewmodel.AddCardCommand.Execute(DeckEditorMTGCardMocker.CreateMTGCardModel());

      Assert.HasCount(1, viewmodel.Cards);
    }

    [TestMethod]
    public void AddCard_Undo_CardRemoved()
    {
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter());

      viewmodel.AddCardCommand.Execute(DeckEditorMTGCardMocker.CreateMTGCardModel());

      Assert.HasCount(1, viewmodel.Cards);

      viewmodel.UndoStack.Undo();

      Assert.IsEmpty(viewmodel.Cards);
    }

    [TestMethod]
    public void AddCard_Redo_CardAddedAgain()
    {
      var viewmodel = new CardListViewModel([], new TestMTGCardImporter());

      viewmodel.AddCardCommand.Execute(DeckEditorMTGCardMocker.CreateMTGCardModel());

      Assert.HasCount(1, viewmodel.Cards);

      viewmodel.UndoStack.Undo();

      Assert.IsEmpty(viewmodel.Cards);

      viewmodel.UndoStack.Redo();

      Assert.HasCount(1, viewmodel.Cards);
    }

    [TestMethod]
    public async Task AddCard_Existing_ConflictConfirmationShown()
    {
      var confirmer = new TestConfirmer<ConfirmationResult>();
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new CardListViewModel([card], new TestMTGCardImporter())
      {
        Confirmers = new()
        {
          AddSingleConflictConfirmer = confirmer
        }
      };

      await viewmodel.AddCardCommand.ExecuteAsync(card);

      ConfirmationAssert.ConfirmationShown(confirmer);
    }
  }
}
