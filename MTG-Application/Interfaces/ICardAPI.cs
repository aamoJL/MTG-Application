using MTGApplication.API;
using MTGApplication.Models;
using System;
using System.Threading.Tasks;

namespace MTGApplication.Interfaces
{
  /// <summary>
  /// Generic card API
  /// </summary>
  /// <typeparam name="T">Card type</typeparam>
  public interface ICardAPI<T>
  {
    /// <summary>
    /// Fetch cards from the API using API search parameters
    /// </summary>
    public Task<T[]> FetchCardsWithParameters(string searchParams, int countLimit = int.MaxValue);

    /// <summary>
    /// Fetch cards from the API using API search uri
    /// </summary>
    public Task<T[]> FetchCardsFromUri(string uri, int countLimit = int.MaxValue);

    /// <summary>
    /// Fetch cards from the API using formatted text
    /// </summary>
    public Task<(T[] Found, int NotFoundCount)> FetchFromString(string importText);
    
    /// <summary>
    /// Fetch cards from the API using <see cref="CardDTO"/> array
    /// </summary>
    public Task<(T[] Found, int NotFoundCount)> FetchFromDTOs(CardDTO[] dtoArray);

    /// <summary>
    /// Returns the API's name that was used to fetch the given card
    /// </summary>
    public static string GetAPIName(MTGCard card)
    {
      if (card.Info.ScryfallId != Guid.Empty) { return ScryfallAPI.APIName; }
      else { return string.Empty; }
    }
  }
}
