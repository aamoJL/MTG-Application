using MTGApplication.Features.DeckEditor.ViewModels;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Exporters;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels;

public class TestDeckEditorDependencies : DeckEditorDependencies
{
  public TestDeckEditorDependencies()
  {
    Repository = new();
    Importer = new();
    EdhrecImporter = new();
    ScryfallImporter = new();
    Exporter = new();
    Notifier = new NotImplementedNotifier();
    NetworkService = new();
  }

  public new TestRepository<MTGCardDeckDTO> Repository { get; init { field = value; base.Repository = value; } }
  public new TestMTGCardImporter Importer { get; init { field = value; base.Importer = value; } }
  public new TestEdhrecImporter EdhrecImporter { get; init { field = value; base.EdhrecImporter = value; } }
  public new TestScryfallImporter ScryfallImporter { get; init { field = value; base.ScryfallImporter = value; } }
  public new TestStringExporter Exporter { get; init { field = value; base.Exporter = value; } }
  public new TestNotifier Notifier { get; init { field = value; base.Notifier = value; } }
  public new TestNetworkService NetworkService { get; init { field = value; base.NetworkService = value; } }
}
