using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.General.Services.Databases.Repositories.DeckRepositoryTests.UseCases;

[TestClass]
public class GetDeck
{
  private TestDeckDTORepository Repository { get; } = new();
  private readonly MTGCardDeckDTO _savedDeck = MTGCardDeckDTOMocker.Mock("Saved Deck");

  public GetDeck() => Repository.ContextFactory?.Populate(_savedDeck);

  [TestMethod(DisplayName = "Should return saved deck with the given name")]
  public async Task Execute_Found_ReturnDeck()
  {
    var result = await new GetDeckDTO(Repository).Execute(_savedDeck.Name);

    Assert.AreEqual(result?.Name, _savedDeck.Name);
  }

  [TestMethod(DisplayName = "Should return null if the deck was not found")]
  public async Task Execute_NotFound_ReturnNull()
  {
    var result = await new GetDeckDTO(Repository).Execute("Unsaved Deck");

    Assert.IsNull(result);
  }
}
