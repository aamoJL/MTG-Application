using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.Interfaces
{
  /// <summary>
  /// Interface for database repositories
  /// </summary>
  public interface IRepository<T>
  {
    /// <summary>
    /// Returns true if a item with the given name exists in the database.
    /// </summary>
    public Task<bool> Exists(string name);

    /// <summary>
    /// Add given item to the database.
    /// </summary>
    public Task<bool> Add(T item);

    /// <summary>
    /// Updates given item in the database
    /// </summary>
    public Task<bool> Update(T item);

    /// <summary>
    /// Adds the given item to the database if it does not exists, otherwise updates the item.
    /// </summary>
    public Task<bool> AddOrUpdate(T item);

    /// <summary>
    /// Removes given item from the database
    /// </summary>
    public Task<bool> Remove(T item);

    /// <summary>
    /// Returns all the items from the database.
    /// </summary>
    public Task<IEnumerable<T>> Get();

    /// <summary>
    /// Returns a item with the given name from the database.
    /// </summary>
    public Task<T> Get(string name);
  }
}
