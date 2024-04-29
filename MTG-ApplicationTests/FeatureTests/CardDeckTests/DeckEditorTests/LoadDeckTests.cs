using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardDeck;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

[TestClass]
public class LoadDeckTests
{
  private readonly UseCaseDependencies _dependencies = new();
  private readonly MTGCardDeckDTO _savedDeck = MTGCardDeckDTOMocker.Mock("Deck 1");

  public LoadDeckTests() => _dependencies.ContextFactory.Populate(_savedDeck);

  [TestMethod("Should show load confirmation when executed")]
  [ExpectedException(typeof(ConfirmationException))]
  public async Task Execute_LoadConfirmationShown()
  {
    await new LoadDeck(_dependencies.Repository, _dependencies.CardAPI)
    {
      LoadConfirmation = new TestExceptionConfirmer<string, string[]>()
    }.Execute();
  }

  [TestMethod("Should not show load confirmation when executed with a name")]
  public async Task Execute_WithName_ConfirmationNotShown()
  {
    await new LoadDeck(_dependencies.Repository, _dependencies.CardAPI)
    {
      LoadConfirmation = new TestExceptionConfirmer<string, string[]>()
    }.Execute(_savedDeck.Name);
  }

  [TestMethod("Should return YES and the deck when loading a deck")]
  public async Task Execute_YES_ReturnYesAndDeck()
  {
    var result = await new LoadDeck(_dependencies.Repository, _dependencies.CardAPI)
    {
      LoadConfirmation = new() { OnConfirm = (arg) => Task.FromResult(_savedDeck.Name) }
    }.Execute();

    Assert.AreEqual(ConfirmationResult.Yes, result.ConfirmResult);
    Assert.AreEqual(result.Deck.Name, _savedDeck.Name);
  }

  [TestMethod("Should return FAILURE if the deck was not found")]
  public async Task Execute_DeckNotFound_ReturnFailure()
  {
    var result = await new LoadDeck(_dependencies.Repository, _dependencies.CardAPI)
    {
      LoadConfirmation = new() { OnConfirm = (arg) => Task.FromResult("UnsavedDeck2") }
    }.Execute();

    Assert.AreEqual(ConfirmationResult.Failure, result.ConfirmResult);
    Assert.IsNull(result.Deck, "Deck should not be found");
  }

  [TestMethod("Should return CANCEL if when canceling the load")]
  public async Task Execute_Cancel_ReturnCancel()
  {
    var result = await new LoadDeck(_dependencies.Repository, _dependencies.CardAPI)
    {
      LoadConfirmation = new() { OnConfirm = (arg) => Task.FromResult(string.Empty) }
    }.Execute();

    Assert.AreEqual(ConfirmationResult.Cancel, result.ConfirmResult);
    Assert.IsNull(result.Deck, "Deck should not be found");
  }
}