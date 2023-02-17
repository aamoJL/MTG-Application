using MTGApplication.Models;
using System.Threading.Tasks;

namespace MTGApplication.Interfaces
{
  /// <summary>
  /// Generic card API
  /// </summary>
  /// <typeparam name="T">Card type</typeparam>
  public interface ICardAPI<T>
  { 
    public Task<T[]> FetchCards(string searchParams, int countLimit = 700);
    public Task<(T[] Found, int NotFoundCount)> FetchFromString(string importText);
    public Task<(T[] Found, int NotFoundCount)> FetchFromDTOs(CardDTO[] dtoArray);
  }
}
