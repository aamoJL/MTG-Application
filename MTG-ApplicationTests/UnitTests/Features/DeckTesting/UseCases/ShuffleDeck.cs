using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckTesting.UseCases;

[TestClass]
public class ShuffleDeck
{
  private readonly TestMTGCardImporter _importer = new();

  [TestMethod]
  public void ShuffleDeck_LibraryShuffled()
  {
    var deck = new DeckTestingDeck(
      DeckCards: [.. MTGCardMocker.Mock(count: 100)],
      Commander: null,
      Partner: null);
    var viewmodel = new DeckTestingPageViewModel(deck, _importer);

    foreach (var item in deck.DeckCards)
      viewmodel.Library.Add(new(item.Info));

    var initState = viewmodel.Library.Select(x => x.Info.ScryfallId).ToArray();

    viewmodel.ShuffleDeckCommand.Execute(null);

    CollectionAssert.AreNotEqual(initState, viewmodel.Library.Select(x => x.Info.ScryfallId).ToArray());
  }
}
