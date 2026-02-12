using MTGApplication.Features.CardSearch.ViewModels.SearchPage;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.Features.CardSearch.ViewModels.SearchCard.CardSearchMTGCardViewModel;

namespace MTGApplicationTests.UnitTests.Features.CardSearch.ViewModels.SearchPage;

public class TestSearchPageViewModelFactory
{
  public Worker Worker { get; set; } = new();
  public TestMTGCardImporter Importer { get; set; } = new();
  public TestNotifier Notifier { get; set; } = new();
  public SearchCardConfirmers CardConfirmers { get; set; } = new();

  public CardSearchPageViewModel Build()
  {
    return new()
    {
      Worker = Worker,
      CardConfirmers = CardConfirmers,
      Importer = Importer,
      Notifier = Notifier,
    };
  }
}