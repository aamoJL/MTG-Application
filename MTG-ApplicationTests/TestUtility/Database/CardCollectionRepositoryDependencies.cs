using MTGApplicationTests.TestUtility.Importers;

namespace MTGApplicationTests.TestUtility.Database;

public class CardCollectionRepositoryDependencies
{
  public CardCollectionRepositoryDependencies()
  {
    Repository = new TestCardCollectionDTORepository();
    Importer = new TestMTGCardImporter_old();
  }

  public TestCardCollectionDTORepository Repository { get; }
  public TestMTGCardImporter_old Importer { get; }
}
