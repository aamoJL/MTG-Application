using MTGApplication.Features.DeckEditor;
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
#pragma warning disable IDE0017 // Simplify object initialization
    var vm = new DeckEditorViewModel(deck ?? new())
    {
      CardAPI = _dependencies.CardAPI,
      Repository = _dependencies.Repository,
      DeckEditorConfirmers = confirmers ?? new(),
      Notifier = notifier ?? new()
    };
#pragma warning restore IDE0017 // Simplify object initialization

    // Unsaved changes state needs to be se outside of the constructor
    // because setting the deck will set the unsaved state to false
    vm.HasUnsavedChanges = hasUnsavedChanges;

    return vm;
  }
}
