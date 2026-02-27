using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.Models.Converters;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;

namespace MTGApplicationTests.UnitTests.General.Services.Databases.Repositories.DeckRepositoryTests.UseCases;

[TestClass]
public class AddOrUpdateDeck
{
  private TestDeckDTORepository Repository { get; } = new();
  private readonly DeckEditorMTGDeck _savedDeck = new() { Name = "Saved Deck" };

  public AddOrUpdateDeck() => Repository.ContextFactory?.Populate(DeckEditorMTGDeckToDTOConverter.Convert(_savedDeck));

  [TestMethod(DisplayName = "Should return true if the deck was added to the reposiotry")]
  public async Task Execute_Added_ReturnTrue()
  {
    var newDeck = new MTGCardDeckDTO(name: "New Deck");
    var result = await new AddOrUpdateDeckDTO(Repository).Execute((newDeck, "Save Name"));

    Assert.IsTrue(result, "Should have returned true");
  }

  [TestMethod(DisplayName = "Should return false if the deck could not be added to the repository")]
  public async Task Execute_AddFailure_ReturnFalse()
  {
    Repository.AddFailure = true;

    var newDeck = new MTGCardDeckDTO(name: "New Deck");
    var result = await new AddOrUpdateDeckDTO(Repository).Execute((newDeck, "Save Name"));

    Assert.IsFalse(result, "Should have returned false");
  }

  [TestMethod(DisplayName = "Should return true if an existing deck was updated")]
  public async Task Execute_Update_ReturnTrue()
  {
    var dto = DeckEditorMTGDeckToDTOConverter.Convert(_savedDeck);
    var result = await new AddOrUpdateDeckDTO(Repository).Execute((dto, dto.Name));

    Assert.IsTrue(result, "Should have returned true");
  }

  [TestMethod(DisplayName = "Should return false if an existing deck could not be updated")]
  public async Task Execute_UpdateFailure_ReturnFalse()
  {
    Repository.UpdateFailure = true;

    var dto = DeckEditorMTGDeckToDTOConverter.Convert(_savedDeck);
    var result = await new AddOrUpdateDeckDTO(Repository).Execute((dto, dto.Name));

    Assert.IsFalse(result, "Should have returned false");
  }
}