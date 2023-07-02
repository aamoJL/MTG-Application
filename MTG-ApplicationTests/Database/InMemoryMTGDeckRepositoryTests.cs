using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Database.Repositories;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplicationTests.API;

namespace MTGApplicationTests.Database;

[TestClass]
public class InMemoryMTGDeckRepositoryTests : MTGDeckRepositoryTestsBase
{
  public class TestInMemoryMTGDeckRepository : InMemoryMTGDeckRepository, ITestRepository<MTGCardDeck>
  {
    public TestInMemoryMTGDeckRepository(ICardAPI<MTGCard>? cardAPI = default) : base(cardAPI) => CardAPI ??= new TestCardAPI();

    public bool WillFail { get; init; }

    public override async Task<bool> Add(MTGCardDeck item)
    {
      if (WillFail) { return false; }
      return await base.Add(item);
    }
    public override async Task<bool> AddOrUpdate(MTGCardDeck item)
    {
      if (WillFail) { return false; }
      return await base.AddOrUpdate(item);
    }
    public override async Task<bool> Exists(string name)
    {
      if (WillFail) { return false; }
      return await base.Exists(name);
    }
    public override async Task<IEnumerable<MTGCardDeck>> Get()
    {
      if (WillFail) { return new List<MTGCardDeck>(); }
      return await base.Get();
    }
    public override async Task<MTGCardDeck?> Get(string name)
    {
      if (WillFail) { return null; }
      return await base.Get(name);
    }
    public override async Task<bool> Remove(MTGCardDeck item)
    {
      if (WillFail) { return false; }
      return await base.Remove(item);
    }
    public override async Task<bool> Update(MTGCardDeck item)
    {
      if (WillFail) { return false; }
      return await base.Update(item);
    }

    public void Dispose()
    {
      Decks.Clear();
      GC.SuppressFinalize(this);
    }
  }

  protected override ITestRepository<MTGCardDeck> GetRepository() => new TestInMemoryMTGDeckRepository(new TestCardAPI());

  [TestMethod] public override Task ExistsTest() => base.ExistsTest();
  [TestMethod] public override Task AddTest() => base.AddTest();
  [TestMethod] public override Task GetTest() => base.GetTest();
  [TestMethod] public override Task GetTest_Named() => base.GetTest_Named();
  [TestMethod] public override Task RemoveTest() => base.RemoveTest();
  [TestMethod] public override Task UpdateTest() => base.UpdateTest();
  [TestMethod] public override Task AddAndUpdateTest() => base.AddAndUpdateTest();
}
