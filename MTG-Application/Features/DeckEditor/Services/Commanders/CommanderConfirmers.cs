using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.DeckEditor.Services.Commanders;

public class CommanderConfirmers
{
  public Confirmer<DeckEditorMTGCard, IEnumerable<DeckEditorMTGCard>> ChangeCardPrintConfirmer { get; init; } = new();

  public static Confirmation<IEnumerable<DeckEditorMTGCard>> GetChangeCardPrintConfirmation(IEnumerable<DeckEditorMTGCard> data)
  {
    return new(
      Title: "Card prints",
      Message: string.Empty,
      Data: data);
  }
}
