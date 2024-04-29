using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Databases.Repositories.DeckRepository;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.GeneralTests.Services.DatabaseTests.RepositoryTests.DeckRepositoryTests;

[TestClass]
public class GetDecksTests
{
  private readonly RepositoryDependencies _dependencies = new();
  private readonly MTGCardDeckDTO[] _savedDecks = [
    MTGCardDeckDTOMocker.Mock("Deck 1"),
    MTGCardDeckDTOMocker.Mock("Deck 2")
    ];

  public GetDecksTests() => _dependencies.ContextFactory.Populate(_savedDecks);

  [TestMethod("Should return saved decks")]
  public async Task Execute_Found_ReturnDecks()
  {
    var result = await new GetDecks(_dependencies.Repository, _dependencies.CardAPI).Execute();

    CollectionAssert.AreEquivalent(
      _savedDecks.Select(x => x.Name).ToList(),
      result.Select(x => x.Name).ToList());
  }

  [TestMethod("Should return empty list if no decks was found")]
  public async Task Execute_NotFound_ReturnEmpty()
  {
    var dependencies = new RepositoryDependencies();
    var result = await new GetDecks(dependencies.Repository, dependencies.CardAPI).Execute();

    Assert.IsFalse(result.Any(), "Result should not have any decks");
  }
}
