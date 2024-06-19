using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.Features.DeckEditor.Editor.Services.Converters;
using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplicationTests.FeatureTests.DeckEditorTests.ViewModel.DeckEditorViewModelTests;

public partial class DeckEditorViewModelTests
{
  public abstract class DeckEditorViewModelTestsBase
  {
    protected readonly DeckRepositoryDependencies _dependencies = new();
    protected readonly DeckEditorMTGDeck _savedDeck = MTGCardDeckMocker.Mock("Saved Deck");

    public DeckEditorViewModelTestsBase()
      => _dependencies.ContextFactory.Populate(DeckEditorMTGDeckToDTOConverter.Convert(_savedDeck));

    public class Mocker(DeckRepositoryDependencies dependencies)
    {
      public bool HasUnsavedChanges { get; set; } = false;
      public DeckEditorConfirmers Confirmers { get; set; } = new();
      public DeckEditorMTGDeck Deck { get; set; } = new();
      public Notifier Notifier { get; set; } = new();

      public DeckEditorViewModel MockVM()
      {
        return new(
          dependencies.Importer,
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

