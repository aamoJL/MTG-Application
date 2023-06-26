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
  public partial class SQLiteMTGDeckRepositoryTests
  {
    public class TestSQLiteMTGDeckRepository : SQLiteMTGDeckRepository
    {
      public TestSQLiteMTGDeckRepository(ICardAPI<MTGCard> cardAPI, CardDbContextFactory cardDbContextFactory) : base(cardAPI, cardDbContextFactory)
      {
        AppConfig.Initialize();
      }

      public async Task<MTGCardDTO[]> GetCards()
      {
        using var db = cardDbContextFactory.CreateDbContext();
        return await db.MTGCards.ToArrayAsync();
      }
    }

    [TestMethod]
    public async Task AddTest()
    {
      var repo = new TestSQLiteMTGDeckRepository(new TestCardAPI(), new TestCardDbContextFactory());
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
    public async Task AddTest_Commanders()
    {
      var repo = new TestSQLiteMTGDeckRepository(new TestCardAPI(), new TestCardDbContextFactory());
      var commander = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Commander");
      var partner = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Partner");
      var deck = new MTGCardDeck()
      {
        Name = "Firts",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        },
        Commander = commander,
        CommanderPartner = partner,
      };

      await repo.Add(deck);
      Assert.AreEqual(commander.Info.Name, repo.Get(deck.Name).Result.Commander?.Info.Name);
      Assert.AreEqual(partner.Info.Name, repo.Get(deck.Name).Result.CommanderPartner?.Info.Name);
    }

    [TestMethod]
    public async Task ExistsTest()
    {
      var repo = new TestSQLiteMTGDeckRepository(new TestCardAPI(), new TestCardDbContextFactory());
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
      var repo = new TestSQLiteMTGDeckRepository(new TestCardAPI(), new TestCardDbContextFactory());
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
      var repo = new TestSQLiteMTGDeckRepository(new TestCardAPI(), new TestCardDbContextFactory());
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
      var contextFactory = new TestCardDbContextFactory();
      var repo = new TestSQLiteMTGDeckRepository(new TestCardAPI(), contextFactory);
      var deck1 = new MTGCardDeck()
      {
        Name = "Firts",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        },
        Commander = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Commander"),
        CommanderPartner = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Partner")
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

      var dBcontext = contextFactory.CreateDbContext();
      Assert.AreEqual(deck2.DeckCards.Count, dBcontext.MTGCards.Count());
    }

    [TestMethod]
    public async Task UpdateTest()
    {
      var repo = new TestSQLiteMTGDeckRepository(new TestCardAPI(), new TestCardDbContextFactory());
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
    public async Task UpdateTest_Commanders()
    {
      var contextFactory = new TestCardDbContextFactory();
      var repo = new TestSQLiteMTGDeckRepository(new TestCardAPI(), contextFactory);
      var deck = new MTGCardDeck()
      {
        Name = "Firts",
        DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
        {
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second"),
          Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third"),
        }
      };

      await repo.Add(deck);
      
      var commander = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Commander");
      var partner = Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Partner");
      deck.Commander = commander;
      deck.CommanderPartner = partner;

      await repo.Update(deck);
      Assert.AreEqual(commander.Info.Name, (await repo.Get(deck.Name)).Commander?.Info.Name);
      Assert.AreEqual(partner.Info.Name, (await repo.Get(deck.Name)).CommanderPartner?.Info.Name);

      var dBcontext = contextFactory.CreateDbContext();
      Assert.AreEqual(deck.DeckCards.Count + 2, dBcontext.MTGCards.Count());

      deck.CommanderPartner = null!;
      
      await repo.Update(deck);
      Assert.AreEqual(commander.Info.Name, (await repo.Get(deck.Name)).Commander?.Info.Name);
      Assert.AreEqual(null, (await repo.Get(deck.Name)).CommanderPartner?.Info.Name);
      Assert.AreEqual(deck.DeckCards.Count + 1, dBcontext.MTGCards.Count());
    }

    [TestMethod]
    public async Task AddAndUpdateTest()
    {
      var repo = new TestSQLiteMTGDeckRepository(new TestCardAPI(), new TestCardDbContextFactory());
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
