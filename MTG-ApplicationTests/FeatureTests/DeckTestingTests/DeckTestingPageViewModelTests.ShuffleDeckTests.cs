using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckTestingTests;
public partial class DeckTestingPageViewModelTests
{
  [TestClass]
  public class ShuffleDeckTests
  {
    [TestMethod]
    public void ShuffleDeck_LibraryShuffled()
    {
      var deck = new DeckTestingDeck(
        DeckCards: [.. MTGCardMocker.Mock(count: 100)],
        Commander: null,
        Partner: null,
        Tokens: []);
      var viewmodel = new DeckTestingPageViewModel(deck);

      foreach (var item in deck.DeckCards)
        viewmodel.Library.Add(new(item.Info));

      var initState = viewmodel.Library.Select(x => x.Info.ScryfallId).ToArray();

      viewmodel.ShuffleDeckCommand.Execute(null);

      CollectionAssert.AreNotEqual(initState, viewmodel.Library.Select(x => x.Info.ScryfallId).ToArray());
    }
  }
}
