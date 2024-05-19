using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.CardDeckTests.DeckEditorTests;

public abstract class DeckEditorViewModelTestsBase
{
  protected readonly DeckRepositoryDependencies _dependencies = new();
  protected readonly MTGCardDeck _savedDeck = MTGCardDeckMocker.Mock("Saved Deck");

  public DeckEditorViewModelTestsBase()
    => _dependencies.ContextFactory.Populate(new MTGCardDeckDTO(_savedDeck));

  protected DeckEditorViewModel MockVM(
    DeckEditorConfirmers? confirmers = null,
    bool hasUnsavedChanges = false,
    MTGCardDeck? deck = null,
    Notifier? notifier = null)
  {
    return new DeckEditorViewModel(
      cardAPI: _dependencies.CardAPI,
      deck: deck,
      notifier: notifier,
      confirmers)
    {
      Repository = _dependencies.Repository,
      HasUnsavedChanges = hasUnsavedChanges
    };
  }
}
