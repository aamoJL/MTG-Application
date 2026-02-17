using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCard;

public partial class DeckCardViewModel
{
  public static class Confirmations
  {
    public static Confirmation<IEnumerable<MTGCard>> GetChangeCardPrintConfirmation(IEnumerable<MTGCard> data)
    {
      return new(
        Title: "Card prints",
        Message: string.Empty,
        Data: data);
    }
  }
}
