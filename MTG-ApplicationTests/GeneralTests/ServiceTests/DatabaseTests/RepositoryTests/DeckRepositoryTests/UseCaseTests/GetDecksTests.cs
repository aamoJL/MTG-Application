using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.GeneralTests.ServiceTests.DatabaseTests.RepositoryTests.DeckRepositoryTests.UseCaseTests;

[TestClass]
public class GetDecksTests
{
  private readonly DeckRepositoryDependencies _dependencies = new();
  private readonly MTGCardDeckDTO[] _savedDecks = [
    MTGCardDeckDTOMocker.Mock("Deck 1"),
    MTGCardDeckDTOMocker.Mock("Deck 2")
    ];

  public GetDecksTests() => _dependencies.ContextFactory.Populate(_savedDecks);

  [TestMethod("Should return saved decks")]
  public async Task Execute_Found_ReturnDecks()
  {
    var result = await new GetDeckDTOs(_dependencies.Repository).Execute();

    CollectionAssert.AreEquivalent(
      _savedDecks.Select(x => x.Name).ToList(),
      result.Select(x => x.Name).ToList());
  }

  [TestMethod("Should return empty list if no decks was found")]
  public async Task Execute_NotFound_ReturnEmpty()
  {
    var dependencies = new DeckRepositoryDependencies();
    var result = await new GetDeckDTOs(dependencies.Repository).Execute();

    Assert.IsFalse(result.Any(), "Result should not have any decks");
  }
}
