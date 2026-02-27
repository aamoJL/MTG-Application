using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckTesting.UseCases;

[TestClass]
public class StartNewGame
{
  private readonly TestMTGCardImporter _importer = new();

  [TestMethod]
  public void StartNewGame_LibraryResetMinusSeven()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    viewmodel.Library.Add(new(MTGCardInfoMocker.MockInfo()));

    Assert.HasCount(1, viewmodel.Library);

    viewmodel.StartNewGameCommand.Execute(null);

    Assert.HasCount(deck.DeckCards.Count - 7, viewmodel.Library);
  }

  [TestMethod]
  public void StartNewGame_GraveyardReset()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    viewmodel.Graveyard.Add(new(MTGCardInfoMocker.MockInfo()));

    Assert.HasCount(1, viewmodel.Graveyard);

    viewmodel.StartNewGameCommand.Execute(null);

    Assert.IsEmpty(viewmodel.Graveyard);
  }

  [TestMethod]
  public void StartNewGame_ExileReset()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    viewmodel.Exile.Add(new(MTGCardInfoMocker.MockInfo()));

    Assert.HasCount(1, viewmodel.Exile);

    viewmodel.StartNewGameCommand.Execute(null);

    Assert.IsEmpty(viewmodel.Exile);
  }

  [TestMethod]
  public void StartNewGame_HandResetDrawSeven()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    viewmodel.Hand.Add(new(MTGCardInfoMocker.MockInfo()));

    Assert.HasCount(1, viewmodel.Hand);

    viewmodel.StartNewGameCommand.Execute(null);

    Assert.HasCount(7, viewmodel.Hand);
  }

  [TestMethod]
  public void StartNewGame_CommanderReset()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: MTGCardMocker.Mock(),
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    Assert.IsEmpty(viewmodel.CommandZone);

    viewmodel.StartNewGameCommand.Execute(null);

    Assert.HasCount(1, viewmodel.CommandZone);
  }

  [TestMethod]
  public void StartNewGame_PartnerReset()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: null,
      Partner: MTGCardMocker.Mock());
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    Assert.IsEmpty(viewmodel.CommandZone);

    viewmodel.StartNewGameCommand.Execute(null);

    Assert.HasCount(1, viewmodel.CommandZone);
  }

  [TestMethod]
  public void StartNewGame_PlayerHPIs40()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer)
    {
      PlayerHP = 10,
      EnemyHP = 10
    };

    Assert.AreEqual(10, viewmodel.PlayerHP);
    Assert.AreEqual(10, viewmodel.EnemyHP);

    viewmodel.StartNewGameCommand.Execute(null);

    Assert.AreEqual(40, viewmodel.PlayerHP);
    Assert.AreEqual(40, viewmodel.EnemyHP);
  }

  [TestMethod]
  public void StartNewGame_TurnCountIsZero()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer)
    {
      TurnCount = 2
    };

    Assert.AreEqual(2, viewmodel.TurnCount);

    viewmodel.StartNewGameCommand.Execute(null);

    Assert.AreEqual(0, viewmodel.TurnCount);
  }

  [TestMethod]
  public void StartNewGame_NewGameStartedInvoked()
  {
    var invoked = false;
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    viewmodel.NewGameStarted += () =>
    {
      invoked = true;
    };

    viewmodel.StartNewGameCommand.Execute(null);

    Assert.IsTrue(invoked);
  }

  [TestMethod]
  public void StartNewGame_DeckShuffled()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 100)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    viewmodel.StartNewGameCommand.Execute(null);

    var initState = viewmodel.Library.Select(x => x.Info.ScryfallId).ToArray();

    viewmodel.StartNewGameCommand.Execute(null);

    CollectionAssert.AreNotEqual(initState, viewmodel.Library.Select(x => x.Info.ScryfallId).ToArray());
  }
}
