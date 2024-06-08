using MTGApplication.General.Models.Card;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.API.CardAPI;

public abstract class MTGCardAPI : ICardAPI<MTGCardInfo>
{
  public abstract string Name { get; }
  public abstract int PageSize { get; }

  public abstract Task<CardImportResult> FetchCardsWithSearchQuery(string searchParams);
  public abstract Task<CardImportResult> FetchFromString(string importText);
  public abstract Task<CardImportResult> FetchFromUri(string pageUri, bool paperOnly = false, bool fetchAll = false);
}