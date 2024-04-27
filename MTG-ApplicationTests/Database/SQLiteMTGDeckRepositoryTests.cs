using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication;
using MTGApplication.API.CardAPI;
using MTGApplication.Database;
using MTGApplication.Database.Repositories;
using MTGApplication.General.Models.Card;
using MTGApplication.Models;

namespace MTGApplicationTests.Database;

//[TestClass]
//public partial class SQLiteMTGDeckRepositoryTests : MTGDeckRepositoryTestsBase
//{
//  public class TestSQLiteMTGDeckRepository : SQLiteMTGDeckRepository, ITestRepository<MTGCardDeck>
//  {
//    public TestSQLiteMTGDeckRepository(ICardAPI<MTGCard> cardAPI, CardDbContextFactory cardDbContextFactory) : base(cardAPI, cardDbContextFactory)
//      => AppConfig.Initialize();

//    public void Dispose() => GC.SuppressFinalize(this);
//  }

//  protected override ITestRepository<MTGCardDeck> GetRepository() => new TestSQLiteMTGDeckRepository(new TestCardAPI(), new TestCardDbContextFactory());

//  [TestMethod] public override async Task AddTest() => await base.AddTest();
//  [TestMethod] public override async Task AddTest_Commanders() => await base.AddTest_Commanders();
//  [TestMethod] public override async Task ExistsTest() => await base.ExistsTest();
//  [TestMethod] public override async Task GetTest() => await base.GetTest();
//  [TestMethod] public override async Task GetTest_Named() => await base.GetTest_Named();
//  [TestMethod] public override async Task RemoveTest() => await base.RemoveTest();
//  [TestMethod] public override async Task UpdateTest() => await base.UpdateTest();
//  [TestMethod] public override async Task UpdateTest_Commanders() => await base.UpdateTest_Commanders();
//  [TestMethod] public override async Task AddAndUpdateTest() => await base.AddAndUpdateTest();
//}
