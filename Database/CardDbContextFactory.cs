using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Reflection;

namespace MTGApplication.Database
{
  public class CardDbContextFactory
  {
    protected string connectionString;
    private readonly string DbFileName = "database.db";

    public CardDbContextFactory(string connectionString = "")
    {
      if (string.IsNullOrEmpty(connectionString))
      {
        var folder = Path.Join(
          Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
          App.CompanyName,
          Assembly.GetCallingAssembly().GetName().Name);
        Directory.CreateDirectory(folder);
        
        var dbPath = Path.Join(folder, DbFileName);
        
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
