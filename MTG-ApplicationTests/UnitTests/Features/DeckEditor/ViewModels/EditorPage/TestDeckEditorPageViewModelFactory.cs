using MTGApplication.Features.DeckEditor.ViewModels.EditorPage;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.ViewModels.EditorPage;

public class TestDeckEditorPageViewModelFactory
{
  public Worker Worker { get; set; } = new();
  public TestMTGCardImporter Importer { get; set; } = new();
  public TestRepository<MTGCardDeckDTO> Repository { get; set; } = new();
  public TestNotifier Notifier { get; set; } = new NotImplementedNotifier();
  public DeckEditorPageViewModel.EditorPageConfirmers Confirmers { get; set; } = new();

  public DeckEditorPageViewModel Build()
  {
    return new()
    {
      Worker = Worker,
      Importer = Importer,
      Repository = Repository,
      Notifier = Notifier,
      Confirmers = Confirmers,
    };
  }
}
