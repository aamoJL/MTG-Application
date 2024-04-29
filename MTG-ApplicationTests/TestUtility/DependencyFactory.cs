using MTGApplicationTests.API;
using MTGApplicationTests.Database;

namespace MTGApplicationTests.TestUtility;

public class UseCaseDependencies
{
  public UseCaseDependencies()
  {
    ContextFactory = new();
    Repository = new TestDeckDTORepository(ContextFactory);
    CardAPI = new TestCardAPI();
  }

  public TestCardDbContextFactory ContextFactory { get; }
  public TestDeckDTORepository Repository { get; }
  public TestCardAPI CardAPI { get; }
}
