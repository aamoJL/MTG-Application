using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.DeckEditor.Services.Commanders;

public class CommanderConfirmers
{
  public Confirmer<MTGCardInfo, IEnumerable<MTGCardInfo>> ChangeCardPrintConfirmer { get; init; } = new();

  public static Confirmation<IEnumerable<MTGCardInfo>> GetChangeCardPrintConfirmation(IEnumerable<MTGCardInfo> data)
  {
    return new(
      Title: "Card prints",
      Message: string.Empty,
      Data: data);
  }
}
