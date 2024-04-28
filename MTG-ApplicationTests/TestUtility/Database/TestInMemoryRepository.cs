using MTGApplication;
using MTGApplication.General.Databases.Repositories.MTGDeckRepository;
using MTGApplicationTests.Database;

namespace MTGApplicationTests.TestUtility;
public class TestSQLiteMTGDeckRepository : DeckDTORepository, IDisposable
{
  public TestSQLiteMTGDeckRepository(TestCardDbContextFactory ctxFactory) : base(ctxFactory)
    => AppConfig.Initialize();

  public void Dispose() => GC.SuppressFinalize(this);
}
