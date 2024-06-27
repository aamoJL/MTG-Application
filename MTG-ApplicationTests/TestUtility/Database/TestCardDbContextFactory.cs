using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MTGApplication.General.Services.Databases.Context;

namespace MTGApplicationTests.TestUtility.Database;

public class TestCardDbContextFactory : CardDbContextFactory, IDisposable
{
  private const string _inMemoryConnectionString = "Filename=:memory:";
  private readonly SqliteConnection _connection;

  public TestCardDbContextFactory()
  {
    _connection = new SqliteConnection(_inMemoryConnectionString);
    _connection.Open();
  }

  public override CardDbContext CreateDbContext()
  {
    var options = new DbContextOptionsBuilder<CardDbContext>().UseSqlite(_connection).Options;
    var cardDbContext = new CardDbContext(options);
    cardDbContext.Database.EnsureCreated();
    return cardDbContext;
  }

  public void Populate(IEnumerable<object> entities)
  {
    using var db = CreateDbContext();
    db.AddRange(entities);
    db.SaveChanges();
  }

  public void Populate(object entity)
  {
    using var db = CreateDbContext();
    db.Add(entity);
    db.SaveChanges();
  }

  public void Dispose()
  {
    _connection.Close();
    GC.SuppressFinalize(this);
  }
}

