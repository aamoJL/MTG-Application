using MTGApplication.Models;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MTGApplication.API
{
  /// <summary>
  /// Base class for MTG Card APIs.
  /// </summary>
  public abstract class MTGCardAPI
  {
    public abstract Task<MTGCardModel[]> FetchCards(string searchParams, int pageLimit = 3);
    public abstract Task<MTGCardModel[]> FetchCollectionAsync(string identifiersJson);
    public abstract Task<bool> OpenAPICardWebsite(MTGCardModel card);

    public abstract MTGCardModel GetMTGCardModelFromJson(JsonNode cardObject);
    public abstract string GetFaceUri(string id, bool back);
    public abstract string GetSetIconUri(string setCode);
  }
}
