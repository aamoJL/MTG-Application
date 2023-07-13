using MTGApplication.Models.DTOs;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Interfaces;

/// <summary>
/// Generic card API
/// </summary>
/// <typeparam name="T">Card type</typeparam>
public interface ICardAPI<T>
{
  /// <summary>
  /// API fetch result
  /// </summary>
  public class Result
  {
    public Result(T[] found, int notFoundCount, int totalCount, string nextPageUri = "")
    {
      Found = found;
      NotFoundCount = notFoundCount;
      TotalCount = totalCount;
      NextPageUri = nextPageUri;
    }

    public T[] Found { get; set; }
    public int NotFoundCount { get; set; }
    public string NextPageUri { get; set; }
    /// <summary>
    /// Total item count across all pages
    /// </summary>
    public int TotalCount { get; set; }

    public static Result Empty() => new(Array.Empty<T>(), 0, 0);
  }

  /// <summary>
  /// How many cards the API returns in one query
  /// </summary>
  public int PageSize { get; }

  /// <summary>
  /// Returns API search URI with the <paramref name="searchParams"/>
  /// </summary>
  public string GetSearchUri(string searchParams);

  /// <summary>
  /// Fetch cards from the API using API search parameters
  /// </summary>
  public Task<Result> FetchCardsWithParameters(string searchParams);

  /// <summary>
  /// Fetches cards from the given <paramref name="pageUri"/>
  /// </summary>
  /// <param name="paperOnly">Fetches only cards that are printed on paper</param>
  public Task<Result> FetchFromUri(string pageUri, bool paperOnly = false);

  /// <summary>
  /// Fetch cards from the API using formatted text.
  /// The text formatting depends on the API implementation
  /// </summary>
  public Task<Result> FetchFromString(string importText);

  /// <summary>
  /// Fetch cards from the API using <see cref="CardDTO"/> array
  /// </summary>
  public Task<Result> FetchFromDTOs(CardDTO[] dtoArray);
}
