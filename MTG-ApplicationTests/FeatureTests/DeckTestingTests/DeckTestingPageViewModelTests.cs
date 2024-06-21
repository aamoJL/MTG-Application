﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckTestingTests;
public class DeckTestingPageViewModelTests
{
  [TestClass]
  public class StartNewGameTests
  {
    [TestMethod]
    public void StartNewGame_LibraryResetMinusSeven()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: null,
        Partner: null,
        Tokens: []);
      var viewmodel = new DeckTestingPageViewModel(deck);

      viewmodel.Library.Add(new(MTGCardInfoMocker.MockInfo()));

      Assert.AreEqual(1, viewmodel.Library.Count);

      viewmodel.StartNewGameCommand.Execute(null);

      Assert.AreEqual(deck.DeckCards.Count - 7, viewmodel.Library.Count);
    }

    [TestMethod]
    public void StartNewGame_GraveyardReset()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: null,
        Partner: null,
        Tokens: []);
      var viewmodel = new DeckTestingPageViewModel(deck);

      viewmodel.Graveyard.Add(new(MTGCardInfoMocker.MockInfo()));

      Assert.AreEqual(1, viewmodel.Graveyard.Count);

      viewmodel.StartNewGameCommand.Execute(null);

      Assert.AreEqual(0, viewmodel.Graveyard.Count);
    }

    [TestMethod]
    public void StartNewGame_ExileReset()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: null,
        Partner: null,
        Tokens: []);
      var viewmodel = new DeckTestingPageViewModel(deck);

      viewmodel.Exile.Add(new(MTGCardInfoMocker.MockInfo()));

      Assert.AreEqual(1, viewmodel.Exile.Count);

      viewmodel.StartNewGameCommand.Execute(null);

      Assert.AreEqual(0, viewmodel.Exile.Count);
    }

    [TestMethod]
    public void StartNewGame_HandResetDrawSeven()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: null,
        Partner: null,
        Tokens: []);
      var viewmodel = new DeckTestingPageViewModel(deck);

      viewmodel.Hand.Add(new(MTGCardInfoMocker.MockInfo()));

      Assert.AreEqual(1, viewmodel.Hand.Count);

      viewmodel.StartNewGameCommand.Execute(null);

      Assert.AreEqual(7, viewmodel.Hand.Count);
    }

    [TestMethod]
    public void StartNewGame_CommanderReset()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: MTGCardMocker.Mock(),
        Partner: null,
        Tokens: []);
      var viewmodel = new DeckTestingPageViewModel(deck);

      Assert.AreEqual(0, viewmodel.CommandZone.Count);

      viewmodel.StartNewGameCommand.Execute(null);

      Assert.AreEqual(1, viewmodel.CommandZone.Count);
    }

    [TestMethod]
    public void StartNewGame_PartnerReset()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: null,
        Partner: MTGCardMocker.Mock(),
        Tokens: []);
      var viewmodel = new DeckTestingPageViewModel(deck);

      Assert.AreEqual(0, viewmodel.CommandZone.Count);

      viewmodel.StartNewGameCommand.Execute(null);

      Assert.AreEqual(1, viewmodel.CommandZone.Count);
    }

    [TestMethod]
    public void StartNewGame_PlayerHPIs40()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: null,
        Partner: null,
        Tokens: []);
      var viewmodel = new DeckTestingPageViewModel(deck)
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
        Partner: null,
        Tokens: []);
      var viewmodel = new DeckTestingPageViewModel(deck)
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
        Partner: null,
        Tokens: []);
      var viewmodel = new DeckTestingPageViewModel(deck);

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
        Partner: null,
        Tokens: []);
      var viewmodel = new DeckTestingPageViewModel(deck);

      viewmodel.StartNewGameCommand.Execute(null);

      var initState = viewmodel.Library.Select(x => x.Info.ScryfallId).ToArray();

      viewmodel.StartNewGameCommand.Execute(null);

      CollectionAssert.AreNotEqual(initState, viewmodel.Library.Select(x => x.Info.ScryfallId).ToArray());
    }
  }
}
