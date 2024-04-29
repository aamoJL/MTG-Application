using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardDeck;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.Features.DeckSelector;
[TestClass]
public class GetDeckNamesAndImageUrisTests
{
  private readonly RepositoryDependencies _dependensies = new();

  [TestMethod("Should return names and image URIs for the repository decks")]
  public async Task Execute_HasDecks_ReturnDeckNamesAndImageUris()
  {
    _dependensies.ContextFactory.Populate(new List<MTGCardDeckDTO>
    {
      MTGCardDeckDTOMocker.Mock("Deck 1"),
      MTGCardDeckDTOMocker.Mock("Deck 2"),
      MTGCardDeckDTOMocker.Mock("Deck 3", mockCommander: false),
    });

    var result = await new GetDeckSelectorListItems(_dependensies.Repository, _dependensies.CardAPI).Execute();

    Assert.AreEqual(3, result.Count());
    Assert.AreEqual(2, result.Where(x => !string.IsNullOrEmpty(x.ImageUri)).Count());
  }

  [TestMethod("Should return an empty list if the repository has no decks")]
  public async Task Execute_HasNoDecks_ReturnEmptyList()
  {
    var result = await new GetDeckSelectorListItems(_dependensies.Repository, _dependensies.CardAPI).Execute();

    Assert.IsFalse(result.Any(), "Result should be empty");
  }
}
