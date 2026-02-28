using MTGApplication.Features.CardCollectionEditor.ViewModels.EditorPage;
using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Exporters;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.EditorPage;

public class TestEditorPageViewModelFactory
{
  public CardCollectionEditorPageViewModel.CollectionEditorPageConfirmers EditorPageConfirmers { get; set; } = new();
  public TestMTGCardImporter Importer { get; set; } = new();
  public TestStringExporter Exporter { get; set; } = new();
  public TestRepository<MTGCardCollectionDTO> Repository { get; set; } = new();
  public TestNotifier Notifier { get; set; } = new NotImplementedNotifier();

  public CardCollectionEditorPageViewModel Build()
  {
    return new()
    {
      Repository = Repository,
      Importer = Importer,
      Notifier = Notifier,
      Exporter = Exporter,
      Confirmers = EditorPageConfirmers,
    };
  }
}
