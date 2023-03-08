using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.Interfaces
{
  /// <summary>
  /// Interface for a generic card deck repository.
  /// </summary>
  public interface IDeckRepository<T>
  {
    /// <summary>
    /// Returns true if a deck with the given name exists in the database.
    /// </summary>
    public Task<bool> Exists(string name);
    
    /// <summary>
    /// Add given deck to the database.
    /// </summary>
    public Task<bool> Add(T deck);
    
    /// <summary>
    /// Updates given deck in the database
    /// </summary>
    public Task<bool> Update(T deck);
    
    /// <summary>
    /// Adds the given deck to the database if it does not exists, otherwise updates the deck.
    /// </summary>
    public Task<bool> AddOrUpdate(T deck);
    
    /// <summary>
    /// Removes given deck from the database
    /// </summary>
    public Task<bool> Remove(T deck);
    
    /// <summary>
    /// Returns all the decks from the database.
    /// </summary>
    public Task<IEnumerable<T>> Get();
    
    /// <summary>
    /// Returns a deck with the given name from the database.
    /// </summary>
    public Task<T> Get(string name);
  }
}
