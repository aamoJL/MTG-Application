using MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionCard;
using MTGApplication.General.Models;
using MTGApplication.General.ViewModels;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Services;
using static MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionCard.CardCollectionMTGCardViewModel;

namespace MTGApplicationTests.UnitTests.Features.CardCollectionEditor.ViewModels.CollectionCard;

public class TestCollectionCardViewModelFactory
{
  public Worker Worker { get; set; } = new();
  public CollectionCardConfirmers CardConfirmers { get; set; } = new();
  public TestMTGCardImporter Importer { get; set; } = new();
  public TestNotifier Notifier { get; set; } = new();
  public TestNetworkService NetworkService { get; set; } = new();
  public bool IsOwned { get; set; } = false;

  public CardCollectionMTGCardViewModel Build(MTGCard card)
  {
    return new(card)
    {
      Worker = Worker,
      Confirmers = CardConfirmers,
      Importer = Importer,
      Notifier = Notifier,
      NetworkService = NetworkService,
      IsOwned = IsOwned,
    };
  }
}