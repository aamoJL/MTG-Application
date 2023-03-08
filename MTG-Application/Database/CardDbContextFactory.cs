using Microsoft.EntityFrameworkCore;
using MTGApplication.Services;
using System.IO;

namespace MTGApplication.Database
{
  public class CardDbContextFactory
  {
    protected string connectionString;
    public readonly static string DbFileName = "database.db";

    public CardDbContextFactory(string connectionString = "")
    {
      if (string.IsNullOrEmpty(connectionString))
      {
        var dbPath = Path.Join(IO.GetAppDataPath(), DbFileName);
        connectionString = $"Data Source={dbPath}";
      }
      this.connectionString = connectionString;
    }

    public virtual CardDbContext CreateDbContext()
    {
      DbContextOptions options = new DbContextOptionsBuilder().UseSqlite(connectionString).Options;
      return new CardDbContext(options);
    }
  }
}
