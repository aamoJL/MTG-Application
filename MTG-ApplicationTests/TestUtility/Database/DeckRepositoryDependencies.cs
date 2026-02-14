using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.UnitTests.General.Services.Databases.Repositories.DeckRepositoryTests;

namespace MTGApplicationTests.TestUtility.Database;

[Obsolete]
public class DeckRepositoryDependencies
{
  public DeckRepositoryDependencies()
  {
    Repository = new TestDeckDTORepository();
    Importer = new TestMTGCardImporter_old();
  }

  public InMemoryCardDbContextFactory ContextFactory => field ??= (Repository.DbContextFactory as InMemoryCardDbContextFactory)!;
  public TestDeckDTORepository Repository { get; }
  public TestMTGCardImporter_old Importer { get; }
}