using MTGApplicationTests.TestUtility.Importers;

namespace MTGApplicationTests.TestUtility.Database;

public class DeckRepositoryDependencies
{
  public DeckRepositoryDependencies()
  {
    ContextFactory = new();
    Repository = new TestDeckDTORepository(ContextFactory);
    Importer = new TestMTGCardImporter_old();
  }

  public TestCardDbContextFactory ContextFactory { get; }
  public TestDeckDTORepository Repository { get; }
  public TestMTGCardImporter_old Importer { get; }
}