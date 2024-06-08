using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.CardSearch.Services;

public class CardSearchConfirmers
{
  public Confirmer<DeckEditorMTGCard, IEnumerable<DeckEditorMTGCard>> ShowCardPrintsConfirmer { get; init; } = new();

  public static Confirmation<IEnumerable<DeckEditorMTGCard>> GetShowCardPrintsConfirmation(IEnumerable<DeckEditorMTGCard> data)
  {
    return new(
      Title: "Card prints",
      Message: string.Empty,
      Data: data);
  }
}
