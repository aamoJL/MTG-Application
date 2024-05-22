using MTGApplicationTests.TestUtility.API;

namespace MTGApplicationTests.TestUtility.Database;

public class DeckRepositoryDependencies
{
  public DeckRepositoryDependencies()
  {
    ContextFactory = new();
    Repository = new TestDeckDTORepository(ContextFactory);
    CardAPI = new TestCardAPI();
  }

  public TestCardDbContextFactory ContextFactory { get; }
  public TestDeckDTORepository Repository { get; }
  public TestCardAPI CardAPI { get; }
}
