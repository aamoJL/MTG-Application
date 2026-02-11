using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.NotificationService;
using MTGApplication.General.ViewModels;
using System;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionCard;

public partial class CardCollectionMTGCardViewModel
{
  public class Factory
  {
    public required Worker Worker { get; init; }
    public required Func<MTGCard, bool> IsOwned { get; init; }
    public required CollectionCardConfirmers CardConfirmers { get; init; }
    public required IMTGCardImporter Importer { get; init; }
    public required NotificationService.Notifier Notifier { get; init; }

    public CardCollectionMTGCardViewModel Build(MTGCard card)
    {
      return new(card)
      {
        Worker = Worker,
        Confirmers = CardConfirmers,
        Importer = Importer,
        Notifier = Notifier,
        IsOwned = IsOwned(card),
      };
    }
  }
}
