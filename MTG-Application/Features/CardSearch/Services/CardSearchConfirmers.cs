using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.CardSearch.Services;

public class CardSearchConfirmers
{
  public Confirmer<MTGCardInfo, IEnumerable<MTGCardInfo>> ShowCardPrintsConfirmer { get; init; } = new();

  public static Confirmation<IEnumerable<MTGCardInfo>> GetShowCardPrintsConfirmation(IEnumerable<MTGCardInfo> data)
  {
    return new(
      Title: "Card prints",
      Message: string.Empty,
      Data: data);
  }
}
