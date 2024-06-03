using MTGApplicationTests.TestUtility.API;

namespace MTGApplicationTests.TestUtility.Database;

public class CardCollectionRepositoryDependencies
{
  public CardCollectionRepositoryDependencies()
  {
    ContextFactory = new();
    Repository = new TestCardCollectionDTORepository(ContextFactory);
    CardAPI = new TestCardAPI();
  }

  public TestCardDbContextFactory ContextFactory { get; }
  public TestCardCollectionDTORepository Repository { get; }
  public TestCardAPI CardAPI { get; }
}
