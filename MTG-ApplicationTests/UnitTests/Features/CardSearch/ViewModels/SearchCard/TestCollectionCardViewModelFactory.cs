using MTGApplication.Features.CardSearch.ViewModels.SearchCard;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.Features.CardSearch.ViewModels.SearchCard.CardSearchMTGCardViewModel;

namespace MTGApplicationTests.UnitTests.Features.CardSearch.ViewModels.SearchCard;

public class TestSearchCardViewModelFactory
{
  public Worker Worker { get; set; } = new();
  public SearchCardConfirmers CardConfirmers { get; set; } = new();
  public TestMTGCardImporter Importer { get; set; } = new();
  public TestNotifier Notifier { get; set; } = new();
  public TestNetworkService NetworkService { get; set; } = new();

  public CardSearchMTGCardViewModel Build(MTGCard card)
  {
    return new(card)
    {
      Worker = Worker,
      Confirmers = CardConfirmers,
      Importer = Importer,
      Notifier = Notifier,
      NetworkService = NetworkService,
    };
  }
}