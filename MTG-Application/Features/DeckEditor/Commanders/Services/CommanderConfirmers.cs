using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.DeckEditor.Commanders.Services;

public class CommanderConfirmers
{
  public Confirmer<MTGCard, IEnumerable<MTGCard>> ChangeCardPrintConfirmer { get; init; } = new();

  public static Confirmation<IEnumerable<MTGCard>> GetChangeCardPrintConfirmation(IEnumerable<MTGCard> data)
  {
    return new(
      Title: "Card prints",
      Message: string.Empty,
      Data: data);
  }
}
