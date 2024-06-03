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

    public class Mocker(DeckRepositoryDependencies dependencies)
    {
      public bool HasUnsavedChanges { get; set; } = false;
      public DeckEditorConfirmers Confirmers { get; set; } = new();
      public MTGCardDeck Deck { get; set; } = new();
      public Notifier Notifier { get; set; } = new();

      public DeckEditorViewModel MockVM()
      {
        return new(
          dependencies.CardAPI,
          deck: Deck,
          confirmers: Confirmers,
          notifier: Notifier)
        {
          Repository = dependencies.Repository,
          HasUnsavedChanges = HasUnsavedChanges,
        };
      }
    }
  }
}

