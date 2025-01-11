using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Extensions;
using System.IO;

namespace MTGApplication.General.Services.Databases.Context;

/// <summary>
/// Factory, that returns <see cref="CardDbContext"/>
/// </summary>
public class CardDbContextFactory
{
  public CardDbContextFactory(string connectionString = "")
  {
    if (string.IsNullOrEmpty(connectionString))
    {
      var dbPath = Path.Join(PathExtensions.GetAppDataPath(), DbFileName);
      connectionString = $"Data Source={dbPath}";
    }
    this.connectionString = connectionString;
  }

  protected string connectionString;
  public readonly static string DbFileName = "database.db";

  /// <summary>
  /// Returns <see cref="CardDbContext"/> with the <see cref="connectionString"/> options
  /// </summary>
  public virtual CardDbContext CreateDbContext() => new(new DbContextOptionsBuilder().UseSqlite(connectionString).Options);
}