using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication;
using MTGApplication.Database;
using MTGApplication.Database.Repositories;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplicationTests.API;

namespace MTGApplicationTests.Database;

[TestClass]
public partial class SQLiteMTGDeckRepositoryTests : MTGDeckRepositoryTestsBase
{
  public class TestSQLiteMTGDeckRepository : SQLiteMTGDeckRepository, ITestRepository<MTGCardDeck>
  {
    public TestSQLiteMTGDeckRepository(ICardAPI<MTGCard> cardAPI, CardDbContextFactory cardDbContextFactory) : base(cardAPI, cardDbContextFactory)
      => AppConfig.Initialize();

    public void Dispose() => GC.SuppressFinalize(this);
  }

  protected override ITestRepository<MTGCardDeck> GetRepository() => new TestSQLiteMTGDeckRepository(new TestCardAPI(), new TestCardDbContextFactory());

  [TestMethod] public override Task AddTest() => base.AddTest();
  [TestMethod] public override Task AddTest_Commanders() => base.AddTest_Commanders();
  [TestMethod] public override Task ExistsTest() => base.ExistsTest();
  [TestMethod] public override Task GetTest() => base.GetTest();
  [TestMethod] public override Task GetTest_Named() => base.GetTest_Named();
  [TestMethod] public override Task RemoveTest() => base.RemoveTest();
  [TestMethod] public override Task UpdateTest() => base.UpdateTest();
  [TestMethod] public override Task UpdateTest_Commanders() => base.UpdateTest_Commanders();
  [TestMethod] public override Task AddAndUpdateTest() => base.AddAndUpdateTest();
}
