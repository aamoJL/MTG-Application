using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.Features.DeckSelection.ViewModels.SelectionPage;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;

namespace MTGApplicationTests.UnitTests.Features.DeckSelection.ViewModels.SelectionPage;

public class TestDeckSelectionPageViewModelFactory
{
  public Worker Worker { get; set; } = new();
  public SimpleTestRepository<MTGCardDeckDTO> Repository { get; set; } = new();
  public TestMTGCardImporter Importer { get; set; } = new();
  public TestNotifier Notifier { get; set; } = new();
  public Action<DeckSelectionDeck> OnDeckSelected { get; set; } = null;

  public DeckSelectionPageViewModel Build()
  {
    return new()
    {
      Worker = Worker,
      Repository = Repository,
      Importer = Importer,
      Notifier = Notifier,
      OnSelected = OnDeckSelected
    };
  }
}