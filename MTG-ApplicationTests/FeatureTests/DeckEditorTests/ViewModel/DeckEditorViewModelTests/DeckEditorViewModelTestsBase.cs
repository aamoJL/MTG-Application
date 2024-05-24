using MTGApplication.Features.DeckEditor;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
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
}

