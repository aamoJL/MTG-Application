using MTGApplication.Features.EdhrecSearch.ViewModels;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.Features.CardSearch.ViewModels.SearchCard.CardSearchMTGCardViewModel;

namespace MTGApplicationTests.UnitTests.Features.EdhrecSearch.ViewModels;

public class TestEDHSearchPageViewModelFactory
{
  public Worker Worker { get; set; } = new();
  public TestMTGCardImporter Importer { get; set; } = new();
  public TestEdhrecImporter EdhrecImporter { get; set; } = new TestEdhrecImporter();
  public TestNotifier Notifier { get; set; } = new NotImplementedNotifier();
  public SearchCardConfirmers CardConfirmers { get; set; } = new();

  public EdhrecSearchPageViewModel Build()
  {
    return new()
    {
      Worker = Worker,
      Importer = Importer,
      EdhrecImporter = EdhrecImporter,
      Notifier = Notifier,
      CardConfirmers = CardConfirmers,
    };
  }
}