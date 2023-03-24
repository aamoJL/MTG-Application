using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Database.Repositories;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.Database
{
  [TestClass]
  public class InMemoryMTGDeckRepositoryTests
  {
    public class TestInMemoryMTGDeckRepository : InMemoryMTGDeckRepository, IDisposable
    {
      public TestInMemoryMTGDeckRepository(ICardAPI<MTGCard>? cardAPI = default) : base(cardAPI)
      {
        CardAPI ??= new TestCardAPI();
      }

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

    [TestMethod]
    public async Task ExistsTest()
    {
      var firstDeckName = "First";
      var secondDeckName = "Second";

      using var repo = new TestInMemoryMTGDeckRepository(new TestCardAPI());

      await repo.Add(new MTGCardDeck() { Name = firstDeckName });
      await repo.Add(new MTGCardDeck() { Name = secondDeckName });
      Assert.IsTrue(await repo.Exists(firstDeckName));
      Assert.IsTrue(await repo.Exists(secondDeckName));
      Assert.IsFalse(await repo.Exists("NonExistingName"));
    }

    [TestMethod]
    public async Task AddTest()
    {
      var firstDeckName = "First";
      var secondDeckName = "Second";

      using var repo = new TestInMemoryMTGDeckRepository(new TestCardAPI());
      
      await repo.Add(new MTGCardDeck() { Name = firstDeckName });
      await repo.Add(new MTGCardDeck() { Name = secondDeckName });
      await repo.Add(new MTGCardDeck() { Name = firstDeckName }); // Same as first
      Assert.AreEqual(2, repo.Get().Result.ToList().Count);
    }

    [TestMethod]
    public async Task GetTest()
    {
      var firstDeckName = "First";
      var secondDeckName = "Second";

      using var repo = new TestInMemoryMTGDeckRepository(new TestCardAPI());
      
      await repo.Add(new MTGCardDeck() { Name = firstDeckName });
      await repo.Add(new MTGCardDeck() { Name = secondDeckName });
      
      var decks = repo.Get().Result.ToList();
      Assert.AreEqual(2, decks.Count);
      Assert.AreEqual(firstDeckName, decks[0].Name);
      Assert.AreEqual(secondDeckName, decks[1].Name);
    }

    [TestMethod]
    public async Task GetTest_Named()
    {
      var firstDeckName = "First";
      var secondDeckName = "Second";

      using var repo = new TestInMemoryMTGDeckRepository(new TestCardAPI());

      await repo.Add(new MTGCardDeck() { Name = firstDeckName });
      await repo.Add(new MTGCardDeck() { Name = secondDeckName });

      var deck = await repo.Get(firstDeckName);
      Assert.AreEqual(firstDeckName, deck?.Name);
    }

    [TestMethod]
    public async Task RemoveTest()
    {
      var firstDeckName = "First";
      var secondDeckName = "Second";

      using var repo = new TestInMemoryMTGDeckRepository(new TestCardAPI());

      await repo.Add(new MTGCardDeck() { Name = firstDeckName });
      await repo.Add(new MTGCardDeck() { Name = secondDeckName });
      var firstDeck = await repo.Get(firstDeckName);

      if(firstDeck != null) await repo.Remove(firstDeck);
      Assert.AreEqual(1, repo.Get().Result.ToList().Count);
    }

    [TestMethod]
    public async Task UpdateTest()
    {
      var firstDeckName = "First";

      using var repo = new TestInMemoryMTGDeckRepository(new TestCardAPI());

      var deck = new MTGCardDeck() { Name = firstDeckName };
      await repo.Add(deck);

      // Add
      deck.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 5));
      deck.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 1));
      await repo.Update(deck);
      Assert.AreEqual(6, (await repo.Get(firstDeckName))?.DeckCards.Sum(x => x.Count));

      // Remove
      deck.DeckCards.RemoveAt(0);
      await repo.Update(deck);
      Assert.AreEqual(1, (await repo.Get(firstDeckName))?.DeckCards.Sum(x => x.Count));
    }

    [TestMethod]
    public async Task AddAndUpdateTest()
    {
      var firstDeckName = "First";

      using var repo = new TestInMemoryMTGDeckRepository(new TestCardAPI());

      var deck = new MTGCardDeck() { Name = firstDeckName };
      await repo.AddOrUpdate(deck);

      // Add
      deck.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 5));
      deck.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 1));
      await repo.AddOrUpdate(deck);
      Assert.AreEqual(6, (await repo.Get(firstDeckName))?.DeckCards.Sum(x => x.Count));

      // Remove
      deck.DeckCards.RemoveAt(0);
      await repo.AddOrUpdate(deck);
      Assert.AreEqual(1, (await repo.Get(firstDeckName))?.DeckCards.Sum(x => x.Count));
    }
  }
}
