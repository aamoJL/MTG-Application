using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.CardDeck;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility;
using static MTGApplication.General.Services.ConfirmationService.ConfirmationService;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

public class LoadDeckTests
{
  private readonly UseCaseDependencies _dependencies = new();
  private readonly MTGCardDeck _deck = new() { Name = "Deck" };

  [TestMethod("Should show unsaved confirmation when executed")]
  [ExpectedException(typeof(ConfirmationException))]
  public async Task Execute_UnsavedConfirmShown()
  {
    await new SaveUnsavedChanges(_dependencies.Repository)
    {
      UnsavedChangesConfirmation = new TestExceptionConfirmer<ConfirmationResult>(),
    }.Execute(new(Deck: _deck));
  }
  // TODO: tests
}
