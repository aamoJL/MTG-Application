using MTGApplicationTests.TestUtility.Importers;

namespace MTGApplicationTests.TestUtility.Database;

public class CardCollectionRepositoryDependencies
{
  public CardCollectionRepositoryDependencies()
  {
    ContextFactory = new();
    Repository = new TestCardCollectionDTORepository(ContextFactory);
    Importer = new TestMTGCardImporter();
  }

  public TestCardDbContextFactory ContextFactory { get; }
  public TestCardCollectionDTORepository Repository { get; }
  public TestMTGCardImporter Importer { get; }
}
