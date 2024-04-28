using MTGApplicationTests.API;
using MTGApplicationTests.Database;

namespace MTGApplicationTests.TestUtility;

public class UseCaseDependencies
{
  public UseCaseDependencies()
  {
    ContextFactory = new();
    Repository = new TestSQLiteMTGDeckRepository(ContextFactory);
    CardAPI = new TestCardAPI();
  }

  public TestCardDbContextFactory ContextFactory { get; }
  public TestSQLiteMTGDeckRepository Repository { get; }
  public TestCardAPI CardAPI { get; }
}
