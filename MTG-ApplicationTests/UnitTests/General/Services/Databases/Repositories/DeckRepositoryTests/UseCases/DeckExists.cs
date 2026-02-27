using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.General.Services.Databases.Repositories.DeckRepositoryTests.UseCases;

[TestClass]
public class DeckExists
{
  private TestDeckDTORepository Repository { get; } = new();
  private readonly MTGCardDeckDTO _savedDeck = MTGCardDeckDTOMocker.Mock("Deck");

  public DeckExists() => Repository.ContextFactory?.Populate(_savedDeck);

  [TestMethod(DisplayName = "Should return true if a deck with the given name exists")]
  public async Task Execute_Exists_ReturnTrue()
  {
    var result = await new DeckDTOExists(Repository).Execute(_savedDeck.Name);

    Assert.IsTrue(result, "Should have returned true");
  }

  [TestMethod(DisplayName = "Should return false if a deck with the given name does not exist")]
  public async Task Execute_NotFound_ReturnFalse()
  {
    var result = await new DeckDTOExists(Repository).Execute("Unsaved Deck");

    Assert.IsFalse(result, "Should have returned false");
  }
}