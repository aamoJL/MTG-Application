using MTGApplication.General.Models.Card;

namespace MTGApplication.General.Models;
public interface IMTGCard
{
  public MTGCardInfo Info { get; set; }
}
