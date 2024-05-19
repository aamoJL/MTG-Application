using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class DeckEditorViewModelCommanderTests : DeckEditorViewModelTestsBase
{
  [TestMethod]
  public void SetCommander_ToNull()
  {
    var deck = MTGCardDeckMocker.Mock("Deck", true, true);
    var viewmodel = MockVM(deck: deck);

    viewmodel.CommanderViewModel.ChangeCommand.Execute(null);

    Assert.IsNull(viewmodel.CommanderViewModel.Card);
  }

  [TestMethod]
  public void SetCommander_ToCard()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var deck = MTGCardDeckMocker.Mock("Deck", true, true);
    var viewmodel = MockVM(deck: deck);

    viewmodel.CommanderViewModel.ChangeCommand.Execute(card);

    Assert.AreEqual(card.Info.Name, viewmodel.CommanderViewModel.Card.Info.Name);
  }

  [TestMethod]
  public async Task SetCommander_Import_NotificationSent()
  {
    var vm = MockVM(deck: _savedDeck, notifier: new()
    {
      OnNotify = (arg) => throw new NotificationException(arg)
    });

    await NotificationAssert.NotificationSent(NotificationType.Error,
      () => vm.CommanderViewModel.ImportCommand.ExecuteAsync("null"));
  }

  [TestMethod]
  public void SetPartner_ToNull()
  {
    var deck = MTGCardDeckMocker.Mock("Deck", true, true);
    var viewmodel = MockVM(deck: deck);

    viewmodel.PartnerViewModel.ChangeCommand.Execute(null);

    Assert.IsNull(viewmodel.PartnerViewModel.Card);
  }

  [TestMethod]
  public void SetPartner_ToCard()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var deck = MTGCardDeckMocker.Mock("Deck", true, true);
    var viewmodel = MockVM(deck: deck);

    viewmodel.PartnerViewModel.ChangeCommand.Execute(card);

    Assert.AreEqual(card.Info.Name, viewmodel.PartnerViewModel.Card.Info.Name);
  }

  [TestMethod]
  public void SetCommander_FromNullToCard_Undo_CommanderIsNull()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var deck = MTGCardDeckMocker.Mock("Deck", includeCommander: false);
    var viewmodel = MockVM(deck: deck);

    viewmodel.CommanderViewModel.ChangeCommand.Execute(card);
    viewmodel.UndoCommand.Execute(null);

    Assert.IsNull(viewmodel.CommanderViewModel.Card);
  }

  [TestMethod]
  public void SetCommander_FromNullToCard_Redo_CommanderIsCardAgain()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var deck = MTGCardDeckMocker.Mock("Deck", includeCommander: true);
    var viewmodel = MockVM(deck: deck);

    viewmodel.CommanderViewModel.ChangeCommand.Execute(card);
    viewmodel.UndoCommand.Execute(null);
    viewmodel.RedoCommand.Execute(null);

    Assert.AreEqual(card.Info.Name, viewmodel.CommanderViewModel.Card.Info.Name);
  }

  [TestMethod]
  public void SetPartner_FromNullToCard_Undo_PartnerIsNull()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var deck = MTGCardDeckMocker.Mock("Deck", includeCommander: false);
    var viewmodel = MockVM(deck: deck);

    viewmodel.PartnerViewModel.ChangeCommand.Execute(card);
    viewmodel.UndoCommand.Execute(null);

    Assert.IsNull(viewmodel.PartnerViewModel.Card);
  }

  [TestMethod]
  public void SetPartner_FromNullToCard_Redo_PartnerIsCardAgain()
  {
    var card = Mocker.MTGCardModelMocker.CreateMTGCardModel();
    var deck = MTGCardDeckMocker.Mock("Deck", includeCommander: true);
    var viewmodel = MockVM(deck: deck);

    viewmodel.PartnerViewModel.ChangeCommand.Execute(card);
    viewmodel.UndoCommand.Execute(null);
    viewmodel.RedoCommand.Execute(null);

    Assert.AreEqual(card.Info.Name, viewmodel.PartnerViewModel.Card.Info.Name);
  }
}