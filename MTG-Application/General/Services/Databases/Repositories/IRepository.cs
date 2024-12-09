using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Databases.Repositories;

public static class RepositoryUtilities<T> where T : class
{
  public static Action<DbSet<T>> EmptyIncludes => (T) => { };
}

/// <summary>
/// Interface for database repositories
/// </summary>
public interface IRepository<T> where T : class
{
  /// <summary>
  /// Returns <see langword="true"/> if an item with the given <paramref name="name"/> exists in the database.
  /// </summary>
  public Task<bool> Exists(string name);

  /// <summary>
  /// Adds given <paramref name="item"/> to the database.
  /// </summary>
  public Task<bool> Add(T item);

  /// <summary>
  /// Updates given <paramref name="item"/> in the database
  /// </summary>
  public Task<bool> Update(T item);

  /// <summary>
  /// Adds the given <paramref name="item"/> to the database if it does not exist, otherwise updates the item.
  /// </summary>
  public Task<bool> AddOrUpdate(T item);

  /// <summary>
  /// Removes the given <paramref name="item"/> from the database
  /// </summary>
  public Task<bool> Delete(T item);

  /// <summary>
  /// Returns all the items from the database.
  /// </summary>
  public Task<IEnumerable<T>> Get(Action<DbSet<T>>? setIncludes = null);

  /// <summary>
  /// Returns an item with the given <paramref name="name"/> from the database.
  /// </summary>
  public Task<T?> Get(string name, Action<DbSet<T>>? setIncludes = null);
}
