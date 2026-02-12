using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardSearch.ViewModels.SearchCard;

public partial class CardSearchMTGCardViewModel
{
  public class SearchCardConfirmers
  {
    public Func<Confirmation<IEnumerable<MTGCard>>, Task> ConfirmCardPrints { get => field ?? throw new NotImplementedException(); set; }
  }
}