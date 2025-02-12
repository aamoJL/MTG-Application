using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.CardSearch.Services;

public class CardSearchConfirmers
{
  public DataOnlyConfirmer<IEnumerable<MTGCard>> ShowCardPrintsConfirmer { get; init; } = new();

  public static Confirmation<IEnumerable<MTGCard>> GetShowCardPrintsConfirmation(IEnumerable<MTGCard> data)
  {
    return new(
      Title: "Card prints",
      Message: string.Empty,
      Data: data);
  }
}
