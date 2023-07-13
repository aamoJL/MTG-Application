using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MTGApplication.Database;

namespace MTGApplicationTests.Database;

public class TestCardDbContextFactory : CardDbContextFactory, IDisposable
{
  private const string inMemoryConnectionString = "Filename=:memory:";
  private readonly SqliteConnection connection;

  public TestCardDbContextFactory()
  {
    connection = new SqliteConnection(inMemoryConnectionString);
    connection.Open();
  }

  public override CardDbContext CreateDbContext()
  {
    var options = new DbContextOptionsBuilder<CardDbContext>().UseSqlite(connection).Options;
    var cardDbContext = new CardDbContext(options);
    cardDbContext.Database.EnsureCreated();
    return cardDbContext;
  }

  public void Dispose()
  {
    connection.Close();
    GC.SuppressFinalize(this);
  }
}

