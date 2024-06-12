using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MTGApplication.General.Services.IOServices;
using System.IO;

namespace MTGApplication.General.Services.Databases.Context;

/// <summary>
/// Factory, that is used to create <see cref="CardDbContext"/> when creating Entity Framework Core migrations.
/// Documentation: <seealso href="https://learn.microsoft.com/en-us/ef/core/cli/dbcontext-creation?tabs=dotnet-core-cli"/>
/// </summary>
public class CardDesignTimeDbContextFactory : IDesignTimeDbContextFactory<CardDbContext>
{
  public CardDbContext CreateDbContext(string[] args)
  {
    var dbPath = Path.Join(FileService.GetAppDataPath(), CardDbContextFactory.DbFileName);
    var connectionString = $"Data Source={dbPath}";

    var options = new DbContextOptionsBuilder().UseSqlite(connectionString).Options;
    return new CardDbContext(options);
  }
}