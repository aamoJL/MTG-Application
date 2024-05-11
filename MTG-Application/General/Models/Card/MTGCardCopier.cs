using MTGApplication.General.Services;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.General.Models.Card;

public class MTGCardCopier : IClassCopier<MTGCard>
{
  public MTGCard Copy(MTGCard item) => new(item.Info, item.Count);
  
  public IEnumerable<MTGCard> Copy(IEnumerable<MTGCard> items) 
    => items.Select(x => new MTGCard(x.Info, x.Count));
}