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
    public async Task SetCommander_ToNull()
    {
      var deck = MTGCardDeckMocker.Mock("Deck", true, true);
      var viewmodel = new Mocker(_dependencies) { Deck = deck }.MockVM();

      await viewmodel.CommanderCommands.ChangeCommanderCommand.ExecuteAsync(null);

      Assert.IsNull(viewmodel.Commander);
    }

    [TestMethod]
    public async Task SetCommander_ToCard()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var deck = MTGCardDeckMocker.Mock("Deck", true, true);
      var viewmodel = new Mocker(_dependencies) { Deck = deck }.MockVM();

      await viewmodel.CommanderCommands.ChangeCommanderCommand.ExecuteAsync(card);

      Assert.AreEqual(card.Info.Name, viewmodel.Commander.Info.Name);
    }

    [TestMethod]
    public async Task SetCommander_Import_NotificationSent()
    {
      var notifier = new TestNotifier();
      var viewmodel = new Mocker(_dependencies)
      {
        Deck = _savedDeck,
        Notifier = notifier
      }.MockVM();

      await viewmodel.CommanderCommands.ImportCommanderCommand.ExecuteAsync("null");

      NotificationAssert.NotificationSent(NotificationType.Error, notifier);
    }

    [TestMethod]
    public async Task SetPartner_ToNull()
    {
      var deck = MTGCardDeckMocker.Mock("Deck", true, true);
      var viewmodel = new Mocker(_dependencies) { Deck = deck }.MockVM();

      await viewmodel.PartnerCommands.ChangeCommanderCommand.ExecuteAsync(null);

      Assert.IsNull(viewmodel.Partner);
    }

    [TestMethod]
    public async Task SetPartner_ToCard()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var deck = MTGCardDeckMocker.Mock("Deck", true, true);
      var viewmodel = new Mocker(_dependencies) { Deck = deck }.MockVM();

      await viewmodel.PartnerCommands.ChangeCommanderCommand.ExecuteAsync(card);

      Assert.AreEqual(card.Info.Name, viewmodel.Partner.Info.Name);
    }

    [TestMethod]
    public async Task SetCommander_FromNullToCard_Undo_CommanderIsNull()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var deck = MTGCardDeckMocker.Mock("Deck", includeCommander: false);
      var viewmodel = new Mocker(_dependencies) { Deck = deck }.MockVM();

      await viewmodel.CommanderCommands.ChangeCommanderCommand.ExecuteAsync(card);
      viewmodel.UndoCommand.Execute(null);

      Assert.IsNull(viewmodel.Commander);
    }

    [TestMethod]
    public async Task SetCommander_FromNullToCard_Redo_CommanderIsCardAgain()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var deck = MTGCardDeckMocker.Mock("Deck", includeCommander: true);
      var viewmodel = new Mocker(_dependencies) { Deck = deck }.MockVM();

      await viewmodel.CommanderCommands.ChangeCommanderCommand.ExecuteAsync(card);
      viewmodel.UndoCommand.Execute(null);
      viewmodel.RedoCommand.Execute(null);

      Assert.AreEqual(card.Info.Name, viewmodel.Commander.Info.Name);
    }

    [TestMethod]
    public async Task SetPartner_FromNullToCard_Undo_PartnerIsNull()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var deck = MTGCardDeckMocker.Mock("Deck", includeCommander: false);
      var viewmodel = new Mocker(_dependencies) { Deck = deck }.MockVM();

      await viewmodel.PartnerCommands.ChangeCommanderCommand.ExecuteAsync(card);
      viewmodel.UndoCommand.Execute(null);

      Assert.IsNull(viewmodel.Partner);
    }

    [TestMethod]
    public async Task SetPartner_FromNullToCard_Redo_PartnerIsCardAgain()
    {
      var card = DeckEditorMTGCardMocker.CreateMTGCardModel();
      var deck = MTGCardDeckMocker.Mock("Deck", includeCommander: true);
      var viewmodel = new Mocker(_dependencies) { Deck = deck }.MockVM();

      await viewmodel.PartnerCommands.ChangeCommanderCommand.ExecuteAsync(card);
      viewmodel.UndoCommand.Execute(null);
      viewmodel.RedoCommand.Execute(null);

      Assert.AreEqual(card.Info.Name, viewmodel.Partner.Info.Name);
    }
  }
}

