using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Mocker;
namespace MTGApplicationTests.GeneralTests.Services.DatabaseTests.RepositoryTests.DeckRepositoryTests;

[TestClass]
public class DeckDTORepositoryTests
{
  private readonly DeckRepositoryDependencies _dependencies = new();
  private readonly MTGCardDeckDTO _savedDeck = new("Saved Deck")
  {
    DeckCards = [MTGCardDTOMocker.Mock("first"), MTGCardDTOMocker.Mock("second"), MTGCardDTOMocker.Mock("third")],
    WishlistCards = [MTGCardDTOMocker.Mock("first")],
    MaybelistCards = [MTGCardDTOMocker.Mock("first"), MTGCardDTOMocker.Mock("second")],
    RemovelistCards = [MTGCardDTOMocker.Mock("first")],
    Commander = MTGCardDTOMocker.Mock("Commander"),
    CommanderPartner = MTGCardDTOMocker.Mock("Partner")
  };

  public DeckDTORepositoryTests() => _dependencies.ContextFactory.Populate(_savedDeck);

  [TestMethod]
  public async Task Add()
  {
    var deck = new MTGCardDeckDTO("New Deck")
    {
      DeckCards =
      [
        MTGCardDTOMocker.Mock("first"),
        MTGCardDTOMocker.Mock("second"),
        MTGCardDTOMocker.Mock("third")
      ],
      Commander = MTGCardDTOMocker.Mock("Commander"),
      CommanderPartner = MTGCardDTOMocker.Mock("Partner")
    };

    await _dependencies.Repository.Add(deck);
    await _dependencies.Repository.Add(deck); // Added again, should not be added twice

    Assert.AreEqual(2, (await _dependencies.Repository.Get()).Count(), "Database should have only two decks");

    var dbDeck = await _dependencies.Repository.Get(deck.Name);

    Assert.AreEqual(deck.DeckCards.Count, dbDeck?.DeckCards.Count, "Card cound does not match");
    Assert.AreEqual(deck.Commander.Name, dbDeck?.Commander.Name, "Commanders don't match");
    Assert.AreEqual(deck.CommanderPartner.Name, dbDeck?.CommanderPartner.Name, "Partners don't match");
  }

  [TestMethod]
  public async Task Exists()
  {
    var exists = await _dependencies.Repository.Exists(_savedDeck.Name);

    Assert.IsTrue(exists, "Saved deck should exist in the database");

    var notExists = await _dependencies.Repository.Exists("Unsaved Deck");

    Assert.IsFalse(notExists, "Unsaved deck should not exist in the database");
  }

  [TestMethod]
  public async Task Get()
  {
    var result = await _dependencies.Repository.Get();

    Assert.AreEqual(1, result.Count());
    Assert.AreEqual(_savedDeck.Name, result.First().Name);
  }

  [TestMethod]
  public async Task Get_WithName()
  {
    var result = await _dependencies.Repository.Get(_savedDeck.Name);

    Assert.IsNotNull(result, "Result should not be null");
    Assert.AreEqual(_savedDeck.Name, result.Name, "Names don't match");
    Assert.AreEqual(_savedDeck.DeckCards.Count, result.DeckCards.Count, "Deck card counts don't not match");
    Assert.AreEqual(_savedDeck.MaybelistCards.Count, result.MaybelistCards.Count, "Maybelist card counts don't not match");
    Assert.AreEqual(_savedDeck.WishlistCards.Count, result.WishlistCards.Count, "Wishlist card counts don't not match");
    Assert.AreEqual(_savedDeck.RemovelistCards.Count, result.RemovelistCards.Count, "Remove card counts don't not match");
  }

  [TestMethod]
  public async Task Delete()
  {
    var result = await _dependencies.Repository.Delete(_savedDeck);

    Assert.IsTrue(result, "Result should be true");

    var exists = await _dependencies.Repository.Exists(_savedDeck.Name);

    Assert.IsFalse(exists, "Deck should not exists");
  }

  [TestMethod]
  public async Task Update()
  {
    _savedDeck.WishlistCards.Add(MTGCardDTOMocker.Mock("New card"));
    _savedDeck.DeckCards[0].Count = 3;
    _savedDeck.DeckCards.RemoveAt(1);
    _savedDeck.Commander = MTGCardDTOMocker.Mock("New Commander");
    _savedDeck.CommanderPartner = MTGCardDTOMocker.Mock("New Partner");

    var result = await _dependencies.Repository.Update(_savedDeck);

    Assert.IsTrue(result, "Result should be true");

    var dbDeck = await _dependencies.Repository.Get(_savedDeck.Name);

    Assert.AreEqual(_savedDeck.WishlistCards.Count, dbDeck?.WishlistCards.Count, "Wishlist card counts don't match");
    Assert.AreEqual(_savedDeck.DeckCards.Count, dbDeck?.DeckCards.Count, "Deck card counts don't match");
    Assert.AreEqual(_savedDeck.DeckCards.Sum(x => x.Count), dbDeck?.DeckCards.Sum(x => x.Count), "Deck card sums don't match");
    Assert.AreEqual(_savedDeck.Commander.Name, dbDeck?.Commander.Name, "Commander names don't match");
    Assert.AreEqual(_savedDeck.CommanderPartner.Name, dbDeck?.CommanderPartner.Name, "Partner names don't match");
  }
}
