using MTGApplicationTests.TestUtility.API;

namespace MTGApplicationTests.TestUtility.Database;

public class CollectionRepositoryDependencies
{
  public CollectionRepositoryDependencies()
  {
    ContextFactory = new();
    Repository = new TestCardCollectionDTORepository(ContextFactory);
    CardAPI = new TestCardAPI();
  }

  public TestCardDbContextFactory ContextFactory { get; }
  public TestCardCollectionDTORepository Repository { get; }
  public TestCardAPI CardAPI { get; }
}
