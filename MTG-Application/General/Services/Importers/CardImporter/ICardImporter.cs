using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter;

/// <summary>
/// Generic card API
/// </summary>
public partial interface ICardImporter
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
  public Task<CardImportResult> ImportCardsWithSearchQuery(string searchParams);

  /// <summary>
  /// Fetches cards from the given <paramref name="pageUri"/>
  /// </summary>
  /// <param name="paperOnly">Fetches only cards that are printed on paper</param>
  public Task<CardImportResult> ImportFromUri(string pageUri, bool paperOnly = false, bool fetchAll = false);

  /// <summary>
  /// Fetch cards from the API using formatted text.
  /// The text formatting depends on the API implementation
  /// </summary>
  public Task<CardImportResult> ImportFromString(string importText);
}
