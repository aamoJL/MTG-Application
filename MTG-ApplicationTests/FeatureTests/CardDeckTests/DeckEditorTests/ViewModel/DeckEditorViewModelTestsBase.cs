using MTGApplication.Features.CardDeck;
using MTGApplication.General.Models.CardDeck;
using MTGApplicationTests.Services;
using MTGApplicationTests.TestUtility;

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
    DeckEditorNotifier? notifier = null)
  {
    var vm = new DeckEditorViewModel(deck ?? new())
    {
      CardAPI = _dependencies.CardAPI,
      Repository = _dependencies.Repository,
      Confirmers = confirmers ?? new(),
      Notifier = notifier ?? new()
    };

    vm.HasUnsavedChanges = hasUnsavedChanges;

    return vm;
  }
}
