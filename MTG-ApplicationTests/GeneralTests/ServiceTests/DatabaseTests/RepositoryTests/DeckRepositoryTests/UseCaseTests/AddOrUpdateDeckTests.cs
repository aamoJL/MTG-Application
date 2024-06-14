using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Editor.Services.Converters;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.UseCases;
using MTGApplicationTests.TestUtility.Database;

namespace MTGApplicationTests.GeneralTests.ServiceTests.DatabaseTests.RepositoryTests.DeckRepositoryTests.UseCaseTests;

[TestClass]
public class AddOrUpdateDeckTests
{
  private readonly DeckRepositoryDependencies _dependencies = new();
  private readonly DeckEditorMTGDeck _savedDeck = new() { Name = "Saved Deck" };

  public AddOrUpdateDeckTests() => _dependencies.ContextFactory.Populate(DeckEditorMTGDeckToDTOConverter.Convert(_savedDeck));

  [TestMethod("Should return true if the deck was added to the reposiotry")]
  public async Task Execute_Added_ReturnTrue()
  {
    var newDeck = new MTGCardDeckDTO(name: "New Deck");
    var result = await new AddOrUpdateDeckDTO(_dependencies.Repository).Execute((newDeck, "Save Name"));

    Assert.IsTrue(result, "Should have returned true");
  }

  [TestMethod("Should return false if the deck could not be added to the repository")]
  public async Task Execute_AddFailure_ReturnFalse()
  {
    _dependencies.Repository.AddFailure = true;

    var newDeck = new MTGCardDeckDTO(name: "New Deck");
    var result = await new AddOrUpdateDeckDTO(_dependencies.Repository).Execute((newDeck, "Save Name"));

    Assert.IsFalse(result, "Should have returned false");
  }

  [TestMethod("Should return true if an existing deck was updated")]
  public async Task Execute_Update_ReturnTrue()
  {
    var dto = DeckEditorMTGDeckToDTOConverter.Convert(_savedDeck);
    var result = await new AddOrUpdateDeckDTO(_dependencies.Repository).Execute((dto, dto.Name));

    Assert.IsTrue(result, "Should have returned true");
  }

  [TestMethod("Should return false if an existing deck could not be updated")]
  public async Task Execute_UpdateFailure_ReturnFalse()
  {
    _dependencies.Repository.UpdateFailure = true;

    var dto = DeckEditorMTGDeckToDTOConverter.Convert(_savedDeck);
    var result = await new AddOrUpdateDeckDTO(_dependencies.Repository).Execute((dto, dto.Name));

    Assert.IsFalse(result, "Should have returned false");
  }
}