using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.Database
{
  [TestClass]
  public class InMemoryMTGDeckRepositoryTests
  {
    public class TestInMemoryMTGDeckRepository : IDeckRepository<MTGCardDeck>, IDisposable
    {
      public bool WillFail { get; init; }
      protected static List<MTGCardDeck> Decks { get; } = new();

      public async Task<bool> Add(MTGCardDeck deck)
      {
        if (WillFail) { return false; }
        if (!await Exists(deck.Name))
        {
          Decks.Add(deck);
          return true;
        }
        else { return false; }
      }
      public async Task<bool> AddOrUpdate(MTGCardDeck deck)
      {
        if (WillFail) { return false; }
        if (await Exists(deck.Name)) { return await Update(deck); }
        else { return await Add(deck); }
      }
      public async Task<bool> Exists(string name)
      {
        if (WillFail) { return false; }
        return await Task.Run(() => Decks.FirstOrDefault(x => x.Name == name) != null);
      }
      public async Task<IEnumerable<MTGCardDeck>> Get()
      {
        if (WillFail) { return new List<MTGCardDeck>(); }
        return await Task.Run(() => Decks);
      }
      public async Task<MTGCardDeck> Get(string name)
      {
        if (WillFail) { return null; }
        return await Task.Run(() => Decks.FirstOrDefault(x => x.Name == name));
      }
      public async Task<bool> Remove(MTGCardDeck deck)
      {
        return await Task.Run(() => Decks.Remove(Decks.FirstOrDefault(x => x.Name == deck.Name)));
      }
      public async Task<bool> Update(MTGCardDeck deck)
      {
        if (WillFail) { return false; }
        var index = await Task.Run(() => Decks.FindIndex(x => x.Name == deck.Name));
        if (index >= 0) { Decks[index] = deck; return true; }
        else { return false; }
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

      using var repo = new TestInMemoryMTGDeckRepository();

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

      using var repo = new TestInMemoryMTGDeckRepository();
      
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

      using var repo = new TestInMemoryMTGDeckRepository();
      
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

      using var repo = new TestInMemoryMTGDeckRepository();

      await repo.Add(new MTGCardDeck() { Name = firstDeckName });
      await repo.Add(new MTGCardDeck() { Name = secondDeckName });

      var deck = await repo.Get(firstDeckName);
      Assert.AreEqual(firstDeckName, deck.Name);
    }

    [TestMethod]
    public async Task RemoveTest()
    {
      var firstDeckName = "First";
      var secondDeckName = "Second";

      using var repo = new TestInMemoryMTGDeckRepository();

      await repo.Add(new MTGCardDeck() { Name = firstDeckName });
      await repo.Add(new MTGCardDeck() { Name = secondDeckName });
      var firstDeck = await repo.Get(firstDeckName);

      await repo.Remove(firstDeck);
      Assert.AreEqual(1, repo.Get().Result.ToList().Count);
    }

    [TestMethod]
    public async Task UpdateTest()
    {
      var firstDeckName = "First";

      using var repo = new TestInMemoryMTGDeckRepository();

      var deck = new MTGCardDeck() { Name = firstDeckName };
      await repo.Add(deck);

      // Add
      deck.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 5));
      deck.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 1));
      await repo.Update(deck);
      Assert.AreEqual(6, (await repo.Get(firstDeckName)).DeckCards.Sum(x => x.Count));

      // Remove
      deck.DeckCards.RemoveAt(0);
      await repo.Update(deck);
      Assert.AreEqual(1, (await repo.Get(firstDeckName)).DeckCards.Sum(x => x.Count));
    }

    [TestMethod]
    public async Task AddAndUpdateTest()
    {
      var firstDeckName = "First";

      using var repo = new TestInMemoryMTGDeckRepository();

      var deck = new MTGCardDeck() { Name = firstDeckName };
      await repo.AddOrUpdate(deck);

      // Add
      deck.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 5));
      deck.DeckCards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(count: 1));
      await repo.AddOrUpdate(deck);
      Assert.AreEqual(6, (await repo.Get(firstDeckName)).DeckCards.Sum(x => x.Count));

      // Remove
      deck.DeckCards.RemoveAt(0);
      await repo.AddOrUpdate(deck);
      Assert.AreEqual(1, (await repo.Get(firstDeckName)).DeckCards.Sum(x => x.Count));
    }
  }
}
