//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using MTGApplication.Database.Repositories;
//using MTGApplication.Database;
//using MTGApplication.Interfaces;
//using MTGApplication.Models;
//using MTGApplication;
//using Microsoft.EntityFrameworkCore;
//using MTGApplicationTests.Services;
//using MTGApplicationTests.API;
//using MTGApplication.Models.DTOs;
//using MTGApplication.API.CardAPI;
//using MTGApplication.General.Models.Card;

//namespace MTGApplicationTests.Database
//{
//  [TestClass]
//  public class SQLiteMTGCardCollectionTests
//  {
//    public class TestSQLiteMTGCardCollectionRepository : SQLiteMTGCardCollectionRepository
//    {
//      public TestSQLiteMTGCardCollectionRepository(ICardAPI<MTGCard> cardAPI, CardDbContextFactory cardDbContextFactory) : base(cardAPI, cardDbContextFactory)
//      {
//        AppConfig.Initialize();
//      }

//      public async Task<MTGCardDTO[]> GetCards()
//      {
//        using var db = cardDbContextFactory.CreateDbContext();
//        return await db.MTGCards.ToArrayAsync();
//      }
//      public async Task<MTGCardCollectionListDTO[]> GetCollectionLists()
//      {
//        using var db = cardDbContextFactory.CreateDbContext();
//        return await db.MTGCardCollectionLists.ToArrayAsync();
//      }
//    }

//    [TestMethod]
//    public async Task ExistsTest()
//    {
//      var firstCollectionName = "First";
//      var secondCollectionName = "Second";

//      var repo = new TestSQLiteMTGCardCollectionRepository(new TestCardAPI(), new TestCardDbContextFactory());

//      Assert.IsTrue(await repo.Add(new() { Name = firstCollectionName }));
//      Assert.IsTrue(await repo.Add(new() { Name = secondCollectionName }));
//      Assert.IsTrue(await repo.Exists(firstCollectionName));
//      Assert.IsTrue(await repo.Exists(secondCollectionName));
//      Assert.IsFalse(await repo.Exists("NonExistingName"));
//    }

//    [TestMethod]
//    public async Task AddTest()
//    {
//      var firstCollectionName = "First";
//      var secondCollectionName = "Second";

//      var repo = new TestSQLiteMTGCardCollectionRepository(new TestCardAPI(), new TestCardDbContextFactory());

//      await repo.Add(new() { Name = firstCollectionName });
//      await repo.Add(new() { Name = secondCollectionName });
//      await repo.Add(new() { Name = firstCollectionName }); // Same as first
//      Assert.AreEqual(2, repo.Get().Result.ToList().Count);
//    }

//    [TestMethod]
//    public async Task GetTest()
//    {
//      var firstCollectionName = "First";
//      var secondCollectionName = "Second";

//      var repo = new TestSQLiteMTGCardCollectionRepository(new TestCardAPI(), new TestCardDbContextFactory());

//      await repo.Add(new() { Name = firstCollectionName });
//      await repo.Add(new() { Name = secondCollectionName });

//      var decks = repo.Get().Result.ToList();
//      Assert.AreEqual(2, decks.Count);
//      Assert.AreEqual(firstCollectionName, decks[0].Name);
//      Assert.AreEqual(secondCollectionName, decks[1].Name);
//    }

//    [TestMethod]
//    public async Task GetTest_Named()
//    {
//      var firstCollectionName = "First";
//      var secondCollectionName = "Second";

//      var repo = new TestSQLiteMTGCardCollectionRepository(new TestCardAPI(), new TestCardDbContextFactory());

//      await repo.Add(new() { Name = firstCollectionName });
//      await repo.Add(new() { Name = secondCollectionName });

//      var deck = await repo.Get(firstCollectionName);
//      Assert.AreEqual(firstCollectionName, deck.Name);
//    }

//    [TestMethod]
//    public async Task RemoveTest()
//    {
//      var firstCollectionName = "First";
//      var secondCollectionName = "Second";

//      var repo = new TestSQLiteMTGCardCollectionRepository(new TestCardAPI(), new TestCardDbContextFactory());

//      await repo.Add(new()
//      {
//        Name = firstCollectionName,
//        CollectionLists = new()
//        {
//          new()
//          {
//            Name = "First",
//            Cards = new()
//            {
//              Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First"),
//              Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second"),
//            }
//          }
//        }
//      });
//      await repo.Add(new() { Name = secondCollectionName });
//      var firstCollection = await repo.Get(firstCollectionName);

//      await repo.Delete(firstCollection);
//      Assert.AreEqual(1, (await repo.Get()).ToList().Count);
//      Assert.AreEqual(0, (await repo.GetCards()).ToList().Count);
//    }

//    [TestMethod]
//    public async Task UpdateTest()
//    {
//      var firstCollectionName = "First";
//      var firstListName = "First";
//      var secondListName = "Second";
//      var firstList = new MTGCardCollectionList()
//      {
//        Name = firstListName,
//        Cards = new() { Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 5) }
//      };
//      var secondList = new MTGCardCollectionList()
//      {
//        Name = secondListName,
//        Cards = new() { Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1) }
//      };

//      var repo = new TestSQLiteMTGCardCollectionRepository(new TestCardAPI(), new TestCardDbContextFactory());

//      var collection = new MTGCardCollection()
//      {
//        Name = firstCollectionName,
//        CollectionLists = new() { firstList }
//      };
//      await repo.Add(collection);

//      // Add
//      firstList.Cards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second", count: 1));
//      collection.CollectionLists.Add(secondList);
//      await repo.Update(collection);
//      Assert.AreEqual(6, (await repo.Get(firstCollectionName)).CollectionLists.First(x => x.Name == firstListName).Cards.Sum(x => x.Count));
//      Assert.AreEqual(2, (await repo.Get(firstCollectionName)).CollectionLists.Count);
//      Assert.AreEqual(3, (await repo.GetCards()).ToList().Count);

//      // Update name
//      var updatedName = "Updated name";
//      firstList.Name = updatedName;
//      await repo.Update(collection);
//      Assert.AreEqual(2, (await repo.GetCollectionLists()).Length);
//      Assert.IsTrue((await repo.GetCollectionLists()).FirstOrDefault(x => x.Name == updatedName) is not null);
//      Assert.AreEqual(2, (await repo.Get(firstCollectionName)).CollectionLists.Count);

//      // Remove
//      collection.CollectionLists.RemoveAt(collection.CollectionLists.IndexOf(collection.CollectionLists.First(x => x.Name == updatedName)));
//      await repo.Update(collection);
//      Assert.AreEqual(1, (await repo.Get(firstCollectionName)).CollectionLists.First(x => x.Name == secondListName).Cards.Sum(x => x.Count));
//      Assert.AreEqual(1, (await repo.Get(firstCollectionName)).CollectionLists.Count);
//      Assert.AreEqual(1, (await repo.GetCards()).ToList().Count);
//    }

//    [TestMethod]
//    public async Task AddAndUpdateTest()
//    {
//      var firstCollectionName = "First";
//      var firstListName = "First";
//      var secondListName = "Second";
//      var firstList = new MTGCardCollectionList()
//      {
//        Name = firstListName,
//        Cards = new() { Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 5) }
//      };
//      var secondList = new MTGCardCollectionList()
//      {
//        Name = secondListName,
//        Cards = new() { Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "First", count: 1) }
//      };

//      var repo = new TestSQLiteMTGCardCollectionRepository(new TestCardAPI(), new TestCardDbContextFactory());

//      var collection = new MTGCardCollection()
//      {
//        Name = firstCollectionName,
//        CollectionLists = new() { firstList }
//      };
//      await repo.AddOrUpdate(collection);

//      // Add
//      firstList.Cards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Second", count: 1));
//      collection.CollectionLists.Add(secondList);
//      await repo.AddOrUpdate(collection);
//      Assert.AreEqual(6, (await repo.Get(firstCollectionName)).CollectionLists.First(x => x.Name == firstListName).Cards.Sum(x => x.Count));
//      Assert.AreEqual(2, (await repo.Get(firstCollectionName)).CollectionLists.Count);

//      // Remove
//      collection.CollectionLists.RemoveAt(0);
//      await repo.AddOrUpdate(collection);
//      Assert.AreEqual(1, (await repo.Get(firstCollectionName)).CollectionLists.First(x => x.Name == secondListName).Cards.Sum(x => x.Count));
//      Assert.AreEqual(1, (await repo.Get(firstCollectionName)).CollectionLists.Count);
//    }
//  }
//}
