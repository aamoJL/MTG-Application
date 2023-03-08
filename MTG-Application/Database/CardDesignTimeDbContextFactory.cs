using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MTGApplication.Services;
using System.IO;

namespace MTGApplication.Database
{
  public class CardDesignTimeDbContextFactory : IDesignTimeDbContextFactory<CardDbContext>
  {
    public CardDbContext CreateDbContext(string[] args)
    {
      var dbPath = Path.Join(IO.GetAppDataPath(), CardDbContextFactory.DbFileName);
      var connectionString = $"Data Source={dbPath}";

      DbContextOptions options = new DbContextOptionsBuilder().UseSqlite(connectionString).Options;
      return new CardDbContext(options);
    }
  }
}
