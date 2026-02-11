using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System.Collections.Generic;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionCard;

public partial class CardCollectionMTGCardViewModel
{
  public static class Confirmations
  {
    public static Confirmation<IEnumerable<MTGCard>> GetShowCardPrintsConfirmation(IEnumerable<MTGCard> data)
    {
      return new(
        Title: "Card prints",
        Message: string.Empty,
        Data: data);
    }
  }
}
