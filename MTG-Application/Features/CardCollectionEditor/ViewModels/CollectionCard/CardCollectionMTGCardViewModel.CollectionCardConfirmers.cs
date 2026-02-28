using MTGApplication.General.Models;
using MTGApplication.General.Services.ConfirmationService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.Features.CardCollectionEditor.ViewModels.CollectionCard;

public partial class CardCollectionMTGCardViewModel
{
  public class CollectionCardConfirmers
  {
    public Func<Confirmation<IEnumerable<MTGCard>>, Task> ConfirmCardPrints { get => field ?? throw new NotImplementedException(); set; }
  }
}
