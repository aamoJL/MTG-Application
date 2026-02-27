using MTGApplication.Features.DeckEditor.Models;
using MTGApplication.Features.DeckEditor.ViewModels.Deck;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.Services.ReversibleCommandService;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Exporters;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.Features.DeckEditor.ViewModels.Deck.DeckViewModel;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.Deck;

public class TestDeckViewModelFactory
{
  public DeckEditorMTGDeck Model { get; init; } = new();
  public SaveStatus SaveStatus { get; init; } = new();
  public Worker Worker { get; init; } = new();
  public TestMTGCardImporter Importer { private get; init; } = new();
  public TestEdhrecImporter EdhrecImporter { private get; init; } = new();
  public TestScryfallImporter ScryfallImporter { private get; init; } = new();
  public TestStringExporter Exporter { private get; init; } = new();
  public TestRepository<MTGCardDeckDTO> Repository { private get; init; } = new();
  public TestNetworkService NetworkService { private get; init; } = new();
  public ReversibleCommandStack UndoStack { get; init; } = new();
  public TestNotifier Notifier { get; init; } = new NotImplementedNotifier();
  public DeckConfirmers Confirmers { private get; init; } = new();
  public Func<Task> OnDeckDeleted { get; init; } = () => throw new NotImplementedException(nameof(OnDeckDeleted));

  public DeckViewModel Build()
  {
    return new(Model)
    {
      EditorDependencies = new()
      {
        Worker = Worker,
        Importer = Importer,
        EdhrecImporter = EdhrecImporter,
        ScryfallImporter = ScryfallImporter,
        Exporter = Exporter,
        Repository = Repository,
        NetworkService = NetworkService,
        Notifier = Notifier,
        DeckConfirmers = Confirmers
      },
      SaveStatus = SaveStatus,
      UndoStack = UndoStack,
      OnDeleted = OnDeckDeleted,
    };
  }
}
