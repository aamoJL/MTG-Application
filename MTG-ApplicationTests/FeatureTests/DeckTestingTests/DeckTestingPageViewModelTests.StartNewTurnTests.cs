using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckTestingTests;
public partial class DeckTestingPageViewModelTests
{
  [TestClass]
  public class StartNewTurnTests
  {
    [TestMethod]
    public void StartNewTurn_TurnCountPlusOne()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: null,
        Partner: null);
      var viewmodel = new DeckTestingPageViewModel(deck);

      Assert.AreEqual(0, viewmodel.TurnCount);

      viewmodel.StartNewTurnCommand.Execute(null);

      Assert.AreEqual(1, viewmodel.TurnCount);
    }

    [TestMethod]
    public void StartNewTurn_HandCountPlusOne()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: null,
        Partner: null);
      var viewmodel = new DeckTestingPageViewModel(deck);

      viewmodel.Library.Add(new DeckTestingMTGCard(MTGCardInfoMocker.MockInfo()));

      Assert.AreEqual(0, viewmodel.Hand.Count);

      viewmodel.StartNewTurnCommand.Execute(null);

      Assert.AreEqual(1, viewmodel.Hand.Count);
    }

    [TestMethod]
    public void StartNewTurn_LibraryCountMinusOne()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: null,
        Partner: null);
      var viewmodel = new DeckTestingPageViewModel(deck);

      viewmodel.Library.Add(new DeckTestingMTGCard(MTGCardInfoMocker.MockInfo()));

      Assert.AreEqual(1, viewmodel.Library.Count);

      viewmodel.StartNewTurnCommand.Execute(null);

      Assert.AreEqual(0, viewmodel.Library.Count);
    }

    [TestMethod]
    public void StartNewTurn_NewTurnStartedInvoked()
    {
      var invoked = false;
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 10)],
        Commander: null,
        Partner: null);
      var viewmodel = new DeckTestingPageViewModel(deck);

      viewmodel.NewTurnStarted += () =>
      {
        invoked = true;
      };

      viewmodel.StartNewTurnCommand.Execute(null);

      Assert.IsTrue(invoked);
    }
  }
}
