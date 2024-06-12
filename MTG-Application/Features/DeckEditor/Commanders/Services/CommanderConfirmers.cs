using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.DeckEditor.Commanders.Services;

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
