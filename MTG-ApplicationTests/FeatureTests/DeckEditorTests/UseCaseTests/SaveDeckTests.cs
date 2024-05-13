using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class SaveDeckTests
{
  private readonly DeckRepositoryDependencies _dependencies = new();
  private readonly MTGCardDeck _savedDeck = new() { Name = "SavedDeck" };

  public SaveDeckTests() => _dependencies.ContextFactory.Populate(new MTGCardDeckDTO(_savedDeck));

  [TestMethod("Should return TRUE when saving with the same name")]
  public async Task Execute_SameName_ReturnTrue()
  {
    var result = await new SaveDeck(_dependencies.Repository).Execute(new(_savedDeck, _savedDeck.Name));

    Assert.IsTrue(result);
  }

  [TestMethod("Should return TRUE when overriding existing deck")]
  public async Task Execute_Exists_OverrideTrue_ReturnTrue()
  {
    var newDeck = new MTGCardDeck() { Name = "New Deck" };

    var result = await new SaveDeck(_dependencies.Repository).Execute(new(newDeck, _savedDeck.Name, true));

    Assert.IsTrue(result);
  }

  [TestMethod("Should return FALSE when overriding is false and the deck already exists")]
  public async Task Execute_Exists_OverrideFalse_ReturnFalse()
  {
    var newDeck = new MTGCardDeck() { Name = "New Deck" };

    var result = await new SaveDeck(_dependencies.Repository).Execute(new(newDeck, _savedDeck.Name, false));

    Assert.IsFalse(result);
  }

  [TestMethod("Should return FALSE when the deck could not be saved")]
  public async Task Execute_Failure_ReturnFalse()
  {
    _dependencies.Repository.UpdateFailure = true;

    var result = await new SaveDeck(_dependencies.Repository).Execute(new(_savedDeck, _savedDeck.Name));

    Assert.IsFalse(result);
  }
}