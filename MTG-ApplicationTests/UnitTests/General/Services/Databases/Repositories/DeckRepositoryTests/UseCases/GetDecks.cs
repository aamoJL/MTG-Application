using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.General.Services.Databases.Repositories.DeckRepositoryTests.UseCases;

[TestClass]
public class GetDecks
{
  private TestDeckDTORepository Repository { get; } = new();
  private readonly MTGCardDeckDTO[] _savedDecks = [
    MTGCardDeckDTOMocker.Mock("Deck 1"),
    MTGCardDeckDTOMocker.Mock("Deck 2")
    ];

  public GetDecks() => Repository.ContextFactory?.Populate(_savedDecks);

  [TestMethod(DisplayName = "Should return saved decks")]
  public async Task Execute_Found_ReturnDecks()
  {
    var result = await new GetDeckDTOs(Repository).Execute();

    CollectionAssert.AreEquivalent(
      _savedDecks.Select(x => x.Name).ToList(),
      result.Select(x => x.Name).ToList());
  }

  [TestMethod(DisplayName = "Should return empty list if no decks was found")]
  public async Task Execute_NotFound_ReturnEmpty()
  {
    var _repo = new TestDeckDTORepository();
    var result = await new GetDeckDTOs(_repo).Execute();

    Assert.IsFalse(result.Any(), "Result should not have any decks");
  }
}
