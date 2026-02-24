using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.ViewModels.DeckCard;

public partial class DeckCardViewModel
{
  public class CardConfirmers
  {
    public Func<Confirmation<IEnumerable<MTGCard>>, Task<MTGCard?>> ConfirmCardPrints { get => field ?? throw new NotImplementedException(nameof(ConfirmCardPrints)); set; }
  }
}
