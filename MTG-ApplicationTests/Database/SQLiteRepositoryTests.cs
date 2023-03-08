using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication;
using MTGApplication.Database;
using MTGApplication.Database.Repositories;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplicationTests.API;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.Database
{
  [TestClass]
  public class SQLiteRepositoryTests
  {
    public class TestSQLiteRepository : SQLiteMTGDeckRepository
    {
      public TestSQLiteRepository(ICardAPI<MTGCard> cardAPI, CardDbContextFactory cardDbContextFactory) : base(cardAPI, cardDbContextFactory)
      {
        AppConfig.Init();
      }

      public override Task<bool> Add(MTGCardDeck deck)
      {
        return base.Add(deck);
      }
      public override Task<bool> AddOrUpdate(MTGCardDeck deck)
      {
        return base.AddOrUpdate(deck);
      }
      public override Task<bool> Exists(string name)
      {
        return base.Exists(name);
      }
      public override Task<IEnumerable<MTGCardDeck>> Get()
      {
        return base.Get();
      }
      public override Task<MTGCardDeck> Get(string name)
      {
        return base.Get(name);
      }
      public override Task<bool> Remove(MTGCardDeck deck)
      {
        return base.Remove(deck);
      }
      public override Task<bool> Update(MTGCardDeck deck)
      {
        return base.Update(deck);
      }

      public async Task<CardDTO[]> GetCards()
      {
        using var db = cardDbContextFactory.CreateDbContext();
        return await db.MTGCards.ToArrayAsync();
      }
    }
    public class TestCardDbContextFactory : CardDbContextFactory, IDisposable
    {
      private const string inMemoryConnectionString = "Filename=:memory:";
      private readonly SqliteConnection connection;

      public TestCardDbContextFactory()
      {
        connection = new SqliteConnection(inMemoryConnectionString);
        connection.Open();
      }

      public override CardDbContext CreateDbContext()
      {
        var options = new DbContextOptionsBuilder<CardDbContext>().UseSqlite(connection).Options;
        var cardDbContext = new CardDbContext(options);
        cardDbContext.Database.EnsureCreated();
        return cardDbContext;
      }

      public void Dispose()
      {
        connection.Close();
        GC.SuppressFinalize(this);
      }
    }

    [TestMethod]
    public async Task AddTest()
    {
      var repo = new TestSQLiteRepository(new TestCardAPI(), new TestCardDbContextFactory());
      var deck1 = new MTGCardDeck()
      {
        Name = "Firts",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };
      var deck2 = new MTGCardDeck()
      {
        Name = "Second",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };

      await repo.Add(deck1);
      await repo.Add(deck2);
      await repo.Add(deck1); // Same deck again
      Assert.AreEqual(2, repo.Get().Result.ToList().Count);
    }

    [TestMethod]
    public async Task ExistsTest()
    {
      var repo = new TestSQLiteRepository(new TestCardAPI(), new TestCardDbContextFactory());
      var deck1 = new MTGCardDeck()
      {
        Name = "Firts",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };
      var deck2 = new MTGCardDeck()
      {
        Name = "Second",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };

      await repo.Add(deck1);
      await repo.Add(deck2);

      Assert.IsTrue(await repo.Exists(deck1.Name));
      Assert.IsFalse(await repo.Exists("NonExistingName"));
    }

    [TestMethod]
    public async Task GetTest()
    {
      var repo = new TestSQLiteRepository(new TestCardAPI(), new TestCardDbContextFactory());
      var deck1 = new MTGCardDeck()
      {
        Name = "Firts",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };
      var deck2 = new MTGCardDeck()
      {
        Name = "Second",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };

      await repo.Add(deck1);
      await repo.Add(deck2);

      Assert.AreEqual(2, (await repo.Get()).ToList().Count);
    }

    [TestMethod]
    public async Task GetTest_Named()
    {
      var repo = new TestSQLiteRepository(new TestCardAPI(), new TestCardDbContextFactory());
      var deck1 = new MTGCardDeck()
      {
        Name = "Firts",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };
      var deck2 = new MTGCardDeck()
      {
        Name = "Second",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };

      await repo.Add(deck1);
      await repo.Add(deck2);

      Assert.AreEqual(deck1.Name, (await repo.Get(deck1.Name)).Name);
    }

    [TestMethod]
    public async Task RemoveTest()
    {
      var repo = new TestSQLiteRepository(new TestCardAPI(), new TestCardDbContextFactory());
      var deck1 = new MTGCardDeck()
      {
        Name = "Firts",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };
      var deck2 = new MTGCardDeck()
      {
        Name = "Second",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };

      await repo.Add(deck1);
      await repo.Add(deck2);
      await repo.Remove(deck1);

      Assert.AreEqual(1, (await repo.Get()).ToList().Count);
      Assert.IsFalse(await repo.Exists(deck1.Name));
    }

    [TestMethod]
    public async Task UpdateTest()
    {
      var repo = new TestSQLiteRepository(new TestCardAPI(), new TestCardDbContextFactory());
      var deck1 = new MTGCardDeck()
      {
        Name = "Firts",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };
      var deck2 = new MTGCardDeck()
      {
        Name = "Second",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };

      await repo.Add(deck1);
      await repo.Add(deck2);

      deck1.Wishlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
      deck1.DeckCards[0].Count = 3;
      deck1.DeckCards.RemoveAt(1);
      await repo.Update(deck1);

      Assert.AreEqual(1, (await repo.Get(deck1.Name)).Wishlist.Count);
      Assert.AreEqual(2, (await repo.Get(deck1.Name)).DeckCards.Count);
      Assert.AreEqual(4, (await repo.Get(deck1.Name)).DeckCards.Sum(x => x.Count));
    }

    [TestMethod]
    public async Task AddAndUpdateTest()
    {
      var repo = new TestSQLiteRepository(new TestCardAPI(), new TestCardDbContextFactory());
      var deck1 = new MTGCardDeck()
      {
        Name = "First",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };
      var deck2 = new MTGCardDeck()
      {
        Name = "Second",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };

      await repo.AddOrUpdate(deck1);
      await repo.AddOrUpdate(deck2);

      deck1.Wishlist.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
      deck1.DeckCards[0].Count = 3;
      deck1.DeckCards.RemoveAt(1);
      var expectedCardCount = deck1.DeckCards.Count + deck1.Wishlist.Count + deck2.DeckCards.Count;

      await repo.AddOrUpdate(deck1);
      Assert.AreEqual(2, (await repo.Get()).ToList().Count);
      Assert.AreEqual(1, (await repo.Get(deck1.Name)).Wishlist.Count);
      Assert.AreEqual(2, (await repo.Get(deck1.Name)).DeckCards.Count);
      Assert.AreEqual(4, (await repo.Get(deck1.Name)).DeckCards.Sum(x => x.Count));
      Assert.AreEqual(expectedCardCount, (await repo.GetCards()).Length);
    }
  }
}
