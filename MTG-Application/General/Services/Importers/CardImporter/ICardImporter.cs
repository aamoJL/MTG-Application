using MTGApplication.General.Models.Card;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.API.CardAPI;

/// <summary>
/// Generic card API
/// </summary>
/// <typeparam name="T">Card type</typeparam>
public partial interface ICardImporter<TInfo>
{
  /// <summary>
  /// Name of the API
  /// </summary>
  public string Name { get; }

  /// <summary>
  /// How many cards the API returns in one query
  /// </summary>
  public int PageSize { get; }

  /// <summary>
  /// Fetch cards from the API using API search query
  /// </summary>
  public Task<CardImportResult<TInfo>> ImportCardsWithSearchQuery(string searchParams);

  /// <summary>
  /// Fetches cards from the given <paramref name="pageUri"/>
  /// </summary>
  /// <param name="paperOnly">Fetches only cards that are printed on paper</param>
  public Task<CardImportResult<TInfo>> ImportFromUri(string pageUri, bool paperOnly = false, bool fetchAll = false);

  /// <summary>
  /// Fetch cards from the API using formatted text.
  /// The text formatting depends on the API implementation
  /// </summary>
  public Task<CardImportResult<TInfo>> ImportFromString(string importText);
}
