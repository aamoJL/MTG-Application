﻿using MTGApplication.Models;
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
    public Task<T[]> FetchCards(string searchParams, int countLimit = 700);
    
    /// <summary>
    /// Fetch cards from the API using formatted text
    /// </summary>
    public Task<(T[] Found, int NotFoundCount)> FetchFromString(string importText);
    
    /// <summary>
    /// Fetch cards from the API using <see cref="CardDTO"/> array
    /// </summary>
    public Task<(T[] Found, int NotFoundCount)> FetchFromDTOs(CardDTO[] dtoArray);
  }
}
