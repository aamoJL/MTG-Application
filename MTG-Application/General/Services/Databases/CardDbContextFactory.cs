using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Services.IOService;
using System.IO;

namespace MTGApplication.General.Databases;

/// <summary>
/// Factory, that returns <see cref="CardDbContext"/>
/// </summary>
public class CardDbContextFactory
{
  public CardDbContextFactory(string connectionString = "")
  {
    if (string.IsNullOrEmpty(connectionString))
    {
      var dbPath = Path.Join(FileService.GetAppDataPath(), DbFileName);
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