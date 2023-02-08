using MTGApplication.Models;
using System.Threading.Tasks;
using static MTGApplication.Models.MTGCard;

namespace MTGApplication.API
{
  /// <summary>
  /// Base class for MTG Card APIs.
  /// </summary>
  public abstract class MTGCardAPI
  {
    public abstract Task<MTGCard[]> FetchCards(string searchParams, int countLimit = 700);
    public abstract Task<MTGCard[]> FetchImportedCards(string importText);
    
    public abstract Task<bool> PopulateMTGCardInfosAsync(MTGCard[] cards);
  }
}
