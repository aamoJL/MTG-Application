using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.ViewModels;
using static MTGApplication.General.Services.NotificationService.NotificationService;

namespace MTGApplication.Features.CardSearch.ViewModels.SearchCard;

public partial class CardSearchMTGCardViewModel
{
  public class Factory
  {
    public required Worker Worker { get; init; }
    public required IMTGCardImporter Importer { get; init; }
    public required Notifier Notifier { get; init; }
    public required SearchCardConfirmers CardConfirmers { get; init; }

    public CardSearchMTGCardViewModel Build(MTGCard card)
    {
      return new(card)
      {
        Worker = Worker,
        Confirmers = CardConfirmers,
        Importer = Importer,
        Notifier = Notifier,
      };
    }
  }
}