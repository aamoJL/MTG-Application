using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Models;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.Database;

public abstract class MTGDeckRepositoryTestsBase
{
  protected abstract ITestRepository<MTGCardDeck> GetRepository();

  public virtual async Task AddTest()
  {
    using var repo = GetRepository();
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

  public virtual async Task AddTest_Commanders()
  {
    using var repo = GetRepository();
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

  public virtual async Task ExistsTest()
  {
    using var repo = GetRepository();
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

  public virtual async Task GetTest()
  {
    using var repo = GetRepository();
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

  public virtual async Task GetTest_Named()
  {
    using var repo = GetRepository();
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
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      },
      Maybelist = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      },
      Wishlist = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      },
      Removelist = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      }
    };

    await repo.Add(deck1);
    await repo.Add(deck2);

    Assert.AreEqual(deck1.Name, (await repo.Get(deck1.Name)).Name);

    var dbDeck2 = await repo.Get(deck2.Name);
    Assert.AreEqual(deck2.DeckCards.Count, dbDeck2.DeckCards.Count);
    Assert.AreEqual(deck2.Maybelist.Count, dbDeck2.Maybelist.Count);
    Assert.AreEqual(deck2.Wishlist.Count, dbDeck2.Wishlist.Count);
    Assert.AreEqual(deck2.Removelist.Count, dbDeck2.Removelist.Count);
  }

  public virtual async Task RemoveTest()
  {
    using var repo = GetRepository();
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
  }

  public virtual async Task UpdateTest()
  {
    using var repo = GetRepository();
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

  public virtual async Task UpdateTest_Commanders()
  {
    using var repo = GetRepository();
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

    deck.CommanderPartner = null!;

    await repo.Update(deck);
    Assert.AreEqual(commander.Info.Name, (await repo.Get(deck.Name)).Commander?.Info.Name);
    Assert.AreEqual(null, (await repo.Get(deck.Name)).CommanderPartner?.Info.Name);
  }

  public virtual async Task AddAndUpdateTest()
  {
    using var repo = GetRepository();
    var firstCardGUID = new Guid();
    var deck1 = new MTGCardDeck()
    {
      Name = "First",
      DeckCards = new System.Collections.ObjectModel.ObservableCollection<MTGCard>
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "first", setCode: "neo", collectionNumber: "1", scryfallId: firstCardGUID, oracleId: firstCardGUID),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "second", setCode: "neo", collectionNumber: "2"),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "third", setCode: "neo", collectionNumber: "3"),
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
    // Change first card print
    var firstCard = deck1.DeckCards.First(x => x.Info.Name == "first");
    firstCard.Count = 3;
    firstCard.Info = Mocker.MTGCardModelMocker.CreateMTGCardModel(
      name: "first", setCode: "new", collectionNumber: "1", scryfallId: firstCardGUID, oracleId: firstCardGUID).Info;
    // Remove second card
    deck1.DeckCards.Remove(deck1.DeckCards.First(x => x.Info.Name == "second"));

    await repo.AddOrUpdate(deck1);
    var loadedDeck1 = await repo.Get(deck1.Name);
    Assert.AreEqual(2, (await repo.Get()).ToList().Count);
    Assert.AreEqual(1, loadedDeck1.Wishlist.Count);
    Assert.AreEqual(2, loadedDeck1.DeckCards.Count);
    Assert.AreEqual(4, loadedDeck1.DeckCards.Sum(x => x.Count));
    Assert.AreEqual("new", loadedDeck1.DeckCards.First(x => x.Info.Name == "first").Info.SetCode);
  }
}
