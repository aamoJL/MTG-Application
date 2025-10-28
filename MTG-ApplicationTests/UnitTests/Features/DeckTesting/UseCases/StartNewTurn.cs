using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckTesting.UseCases;

[TestClass]
public class StartNewTurn
{
  private readonly TestMTGCardImporter _importer = new();

  [TestMethod]
  public void StartNewTurn_TurnCountPlusOne()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

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
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    viewmodel.Library.Add(new DeckTestingMTGCard(MTGCardInfoMocker.MockInfo()));

    Assert.IsEmpty(viewmodel.Hand);

    viewmodel.StartNewTurnCommand.Execute(null);

    Assert.HasCount(1, viewmodel.Hand);
  }

  [TestMethod]
  public void StartNewTurn_LibraryCountMinusOne()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    viewmodel.Library.Add(new DeckTestingMTGCard(MTGCardInfoMocker.MockInfo()));

    Assert.HasCount(1, viewmodel.Library);

    viewmodel.StartNewTurnCommand.Execute(null);

    Assert.IsEmpty(viewmodel.Library);
  }

  [TestMethod]
  public void StartNewTurn_NewTurnStartedInvoked()
  {
    var invoked = false;
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 10)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    viewmodel.NewTurnStarted += () =>
    {
      invoked = true;
    };

    viewmodel.StartNewTurnCommand.Execute(null);

    Assert.IsTrue(invoked);
  }
}
