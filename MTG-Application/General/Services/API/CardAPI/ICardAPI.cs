using MTGApplication.General.Models.Card;
using System;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.API.CardAPI;

/// <summary>
/// Generic card API
/// </summary>
/// <typeparam name="T">Card type</typeparam>
public partial interface ICardAPI<T>
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
  public Task<CardImportResult> FetchCardsWithSearchQuery(string searchParams);

  /// <summary>
  /// Fetches cards from the given <paramref name="pageUri"/>
  /// </summary>
  /// <param name="paperOnly">Fetches only cards that are printed on paper</param>
  public Task<CardImportResult> FetchFromUri(string pageUri, bool paperOnly = false);

  /// <summary>
  /// Fetch cards from the API using formatted text.
  /// The text formatting depends on the API implementation
  /// </summary>
  public Task<CardImportResult> FetchFromString(string importText);

  /// <summary>
  /// Fetch cards from the API using <see cref="CardDTO"/> array
  /// </summary>
  public Task<CardImportResult> FetchFromDTOs(CardDTO[] dtoArray);
}
