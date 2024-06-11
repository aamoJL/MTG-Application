using MTGApplication.General.Models;
using MTGApplication.General.Models.Card;

namespace MTGApplication.Features.CardSearch.Models;
public class CardSearchMTGCard(MTGCardInfo info) : IMTGCard
{
  public MTGCardInfo Info { get; set; } = info;
}