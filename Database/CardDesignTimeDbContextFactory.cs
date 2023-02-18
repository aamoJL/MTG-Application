using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.IO;

namespace MTGApplication.Database
{
  public class CardDesignTimeDbContextFactory : IDesignTimeDbContextFactory<CardDbContext>
  {
    public CardDbContext CreateDbContext(string[] args)
    {
      var folder = Environment.SpecialFolder.LocalApplicationData;
      var path = Environment.GetFolderPath(folder);
      var DbFileName = "database.db";
      var dbPath = Path.Join(path, DbFileName);
      var connectionString = $"Data Source={dbPath}";

      DbContextOptions options = new DbContextOptionsBuilder().UseSqlite(connectionString).Options;
      return new CardDbContext(options);
    }
  }
}
