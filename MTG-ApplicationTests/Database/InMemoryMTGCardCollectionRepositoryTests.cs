using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Database.Repositories;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.Database;

[TestClass]
public class InMemoryMTGCardCollectionRepositoryTests 
{
  public class TestInMemoryMTGCardCollectionRepository : InMemoryMTGCardCollectionRepository, IDisposable
  {
    public TestInMemoryMTGCardCollectionRepository(ICardAPI<MTGCard>? cardAPI = default) : base(cardAPI) => CardAPI ??= new TestCardAPI();

    public bool WillFail { get; set; }

    public override async Task<bool> Add(MTGCardCollection item)
    {
      if (WillFail) { return false; }
      return await base.Add(item);
    }

    public override async Task<bool> AddOrUpdate(MTGCardCollection item)
    {
      if (WillFail) { return false; }
      return await base.AddOrUpdate(item);
    }
    
    public override async Task<bool> Exists(string name)
    {
      if (WillFail) { return false; }
      return await base.Exists(name);
    }
    
    public override async Task<IEnumerable<MTGCardCollection>> Get()
    {
      if (WillFail) { return new List<MTGCardCollection>(); }
      return await base.Get();
    }
    
    public override async Task<MTGCardCollection?> Get(string name)
    {
      if (WillFail) { return null; }
      return await base.Get(name);
    }
    
    public override async Task<bool> Delete(MTGCardCollection item) => await base.Delete(item);
    
    public override async Task<bool> Update(MTGCardCollection item)
    {
      if (WillFail) { return false; }
      return await base.Update(item);
    }

    public void Dispose()
    {
      Collections.Clear();
      GC.SuppressFinalize(this);
    }
  }

  [TestMethod]
  public async Task ExistsTest()
  {
    var firstCollectionName = "First";
    var secondCollectionName = "Second";

    using var repo = new TestInMemoryMTGCardCollectionRepository();

    await repo.Add(new() { Name = firstCollectionName });
    await repo.Add(new() { Name = secondCollectionName });
    Assert.IsTrue(await repo.Exists(firstCollectionName));
    Assert.IsTrue(await repo.Exists(secondCollectionName));
    Assert.IsFalse(await repo.Exists("NonExistingName"));
  }

  [TestMethod]
  public async Task AddTest()
  {
    var firstCollectionName = "First";
    var secondCollectionName = "Second";

    using var repo = new TestInMemoryMTGCardCollectionRepository();

    await repo.Add(new() { Name = firstCollectionName });
    await repo.Add(new() { Name = secondCollectionName });
    await repo.Add(new() { Name = firstCollectionName }); // Same as first
    Assert.AreEqual(2, repo.Get().Result.ToList().Count);
  }

  [TestMethod]
  public async Task GetTest()
  {
    var firstCollectionName = "First";
    var secondCollectionName = "Second";

    using var repo = new TestInMemoryMTGCardCollectionRepository();

    await repo.Add(new() { Name = firstCollectionName });
    await repo.Add(new() { Name = secondCollectionName });

    var decks = repo.Get().Result.ToList();
    Assert.AreEqual(2, decks.Count);
    Assert.AreEqual(firstCollectionName, decks[0].Name);
    Assert.AreEqual(secondCollectionName, decks[1].Name);
  }

  [TestMethod]
  public async Task GetTest_Named()
  {
    var firstCollectionName = "First";
    var secondCollectionName = "Second";

    using var repo = new TestInMemoryMTGCardCollectionRepository();

    await repo.Add(new() { Name = firstCollectionName });
    await repo.Add(new() { Name = secondCollectionName });

    var deck = await repo.Get(firstCollectionName);
    Assert.AreEqual(firstCollectionName, deck?.Name);
  }

  [TestMethod]
  public async Task RemoveTest()
  {
    var firstCollectionName = "First";
    var secondCollectionName = "Second";

    using var repo = new TestInMemoryMTGCardCollectionRepository();

    await repo.Add(new() { Name = firstCollectionName });
    await repo.Add(new() { Name = secondCollectionName });
    var firstDeck = await repo.Get(firstCollectionName);

    if(firstDeck != null) await repo.Delete(firstDeck);
    Assert.AreEqual(1, repo.Get().Result.ToList().Count);
  }

  [TestMethod]
  public async Task UpdateTest()
  {
    var firstCollectionName = "First";
    var firstListName = "First";
    var secondListName = "Second";
    var firstList = new MTGCardCollectionList()
    {
      Name = firstListName,
      Cards = new() { Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 5) }
    };
    var secondList = new MTGCardCollectionList()
    {
      Name = secondListName,
      Cards = new() { Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1) }
    };

    using var repo = new TestInMemoryMTGCardCollectionRepository();

    var collection = new MTGCardCollection() 
    { 
      Name = firstCollectionName,
      CollectionLists = new() { firstList }
    };
    await repo.Add(collection);

    // Add
    firstList.Cards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second", count: 1));
    collection.CollectionLists.Add(secondList);
    await repo.Update(collection);
    Assert.AreEqual(6, (await repo.Get(firstCollectionName))?.CollectionLists.First(x => x.Name == firstListName).Cards.Sum(x => x.Count));
    Assert.AreEqual(2, (await repo.Get(firstCollectionName))?.CollectionLists.Count);

    // Remove
    collection.CollectionLists.RemoveAt(0);
    await repo.Update(collection);
    Assert.AreEqual(1, (await repo.Get(firstCollectionName))?.CollectionLists.First(x => x.Name == secondListName).Cards.Sum(x => x.Count));
    Assert.AreEqual(1, (await repo.Get(firstCollectionName))?.CollectionLists.Count);
  }

  [TestMethod]
  public async Task AddAndUpdateTest()
  {
    var firstCollectionName = "First";
    var firstListName = "First";
    var secondListName = "Second";
    var firstList = new MTGCardCollectionList()
    {
      Name = firstListName,
      Cards = new() { Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 5) }
    };
    var secondList = new MTGCardCollectionList()
    {
      Name = secondListName,
      Cards = new() { Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1) }
    };

    using var repo = new TestInMemoryMTGCardCollectionRepository();

    var collection = new MTGCardCollection()
    {
      Name = firstCollectionName,
      CollectionLists = new() { firstList }
    };
    await repo.AddOrUpdate(collection);

    // Add
    firstList.Cards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second", count: 1));
    collection.CollectionLists.Add(secondList);
    await repo.AddOrUpdate(collection);
    Assert.AreEqual(6, (await repo.Get(firstCollectionName))?.CollectionLists.First(x => x.Name == firstListName).Cards.Sum(x => x.Count));
    Assert.AreEqual(2, (await repo.Get(firstCollectionName))?.CollectionLists.Count);

    // Remove
    collection.CollectionLists.RemoveAt(0);
    await repo.AddOrUpdate(collection);
    Assert.AreEqual(1, (await repo.Get(firstCollectionName))?.CollectionLists.First(x => x.Name == secondListName).Cards.Sum(x => x.Count));
    Assert.AreEqual(1, (await repo.Get(firstCollectionName))?.CollectionLists.Count);
  }
}
