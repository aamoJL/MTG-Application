using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.UseCaseTests;

[TestClass]
public class LoadDeckTests
{
  private readonly DeckRepositoryDependencies _dependencies = new();
  private readonly MTGCardDeckDTO _savedDeck = MTGCardDeckDTOMocker.Mock("Deck 1");

  public LoadDeckTests() => _dependencies.ContextFactory.Populate(_savedDeck);

  [TestMethod("Should return the deck when the deck has been loaded")]
  public async Task Execute_Exists_ReturnDeck()
  {
    var result = await new LoadDeck(_dependencies.Repository, _dependencies.CardAPI).Execute(_savedDeck.Name);

    Assert.IsNotNull(result);
    Assert.AreEqual(result.Name, _savedDeck.Name);
  }

  [TestMethod("Should return FALSE if the deck was not found")]
  public async Task Execute_DeckNotFound_ReturnFalse()
  {
    var result = await new LoadDeck(_dependencies.Repository, _dependencies.CardAPI).Execute("UnsavedDeck2");

    Assert.IsNull(result, "Deck should not have been found");
  }
}