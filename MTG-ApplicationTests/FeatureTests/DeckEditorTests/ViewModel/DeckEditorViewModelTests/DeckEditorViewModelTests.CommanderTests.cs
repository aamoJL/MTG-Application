using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.Mocker;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  [TestClass]
  public class CommanderTests : DeckEditorViewModelTestsBase
  {
    [TestMethod]
    public async Task SetCommander_Import_NotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        Notifier = notifier,
      }.MockVM();

      await viewmodel.Commander.ImportCommanderCommand.ExecuteAsync("null");

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task SetCommander_Exists_ToNull_CommanderRemoved()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = new()
        {
          Commander = card
        }
      }.MockVM();

      await viewmodel.Commander.ChangeCommanderCommand.ExecuteAsync(null);

      Assert.IsNull(viewmodel.Commander.Card);

      viewmodel.UndoStack.Undo();

      Assert.AreEqual(card, viewmodel.Commander.Card);

      viewmodel.UndoStack.Redo();

      Assert.IsNull(viewmodel.Commander.Card);
    }

    [TestMethod]
    public async Task SetCommander_Null_ToCard_CommanderChanged()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new Mocker(_dependencies).MockVM();

      await viewmodel.Commander.ChangeCommanderCommand.ExecuteAsync(card);

      Assert.AreEqual(card, viewmodel.Commander.Card);

      viewmodel.UndoStack.Undo();

      Assert.IsNull(viewmodel.Commander.Card);

      viewmodel.UndoStack.Redo();

      Assert.AreEqual(card, viewmodel.Commander.Card);
    }

    [TestMethod]
    public async Task SetPartner_Exists_ToNull_PartnerRemoved()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = new()
        {
          CommanderPartner = card
        }
      }.MockVM();

      await viewmodel.Partner.ChangeCommanderCommand.ExecuteAsync(null);

      Assert.IsNull(viewmodel.Partner.Card);

      viewmodel.UndoStack.Undo();

      Assert.AreEqual(card, viewmodel.Partner.Card);

      viewmodel.UndoStack.Redo();

      Assert.IsNull(viewmodel.Partner.Card);
    }

    [TestMethod]
    public async Task SetPartner_Null_ToCard_PartnerChanged()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var viewmodel = new Mocker(_dependencies).MockVM();

      await viewmodel.Partner.ChangeCommanderCommand.ExecuteAsync(card);

      Assert.AreEqual(card, viewmodel.Partner.Card);

      viewmodel.UndoStack.Undo();

      Assert.IsNull(viewmodel.Partner.Card);

      viewmodel.UndoStack.Redo();

      Assert.AreEqual(card, viewmodel.Partner.Card);
    }
  }
}

