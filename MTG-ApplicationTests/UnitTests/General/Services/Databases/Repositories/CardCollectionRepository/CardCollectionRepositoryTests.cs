using MTGApplication.General.Services.Databases.Repositories.CardCollectionRepository.Models;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.General.Services.Databases.Repositories.CardCollectionRepository;

[TestClass]
public class CardCollectionRepositoryTests
{
  private TestCardCollectionDTORepository Repository { get; } = new TestCardCollectionDTORepository();
  private readonly MTGCardCollectionDTO _savedCollection = new("Saved Collection",
  [
    new("List 1", "Query 1", [MTGCardDTOMocker.Mock("Card")]),
    new("List 2", "Query 2", [MTGCardDTOMocker.Mock("Card")]),
  ]);

  public CardCollectionRepositoryTests() => Repository.ContextFactory?.Populate(_savedCollection);

  [TestMethod]
  public async Task Add()
  {
    var collection = new MTGCardCollectionDTO("New Collection", [
      new("New List", "Query 1", [MTGCardDTOMocker.Mock("Card")]),
    ]);

    await Repository.Add(collection);
    await Repository.Add(collection); // Added again, should not be added twice

    Assert.AreEqual(2, (await Repository.Get()).Count(), "Database should have only two collections");

    var dbCollection = await Repository.Get(collection.Name);

    Assert.AreEqual(collection.Name, dbCollection?.Name);
    Assert.AreEqual(collection.CollectionLists.Count, dbCollection?.CollectionLists.Count);
  }

  [TestMethod]
  public async Task Exists()
  {
    var exists = await Repository.Exists(_savedCollection.Name);

    Assert.IsTrue(exists, "Saved collection should exist in the database");

    var notExists = await Repository.Exists("Unsaved Deck");

    Assert.IsFalse(notExists, "Unsaved collection should not exist in the database");
  }

  [TestMethod]
  public async Task Get()
  {
    var result = await Repository.Get();

    Assert.AreEqual(1, result.Count());
    Assert.AreEqual(_savedCollection.Name, result.First().Name);
    Assert.HasCount(_savedCollection.CollectionLists.Count, result.First().CollectionLists);
  }

  [TestMethod]
  public async Task Get_WithName()
  {
    var result = await Repository.Get(_savedCollection.Name);

    Assert.IsNotNull(result, "Result should not be null");
    Assert.AreEqual(_savedCollection.Name, result.Name, "Names don't match");
    Assert.HasCount(_savedCollection.CollectionLists.Count, result.CollectionLists);
  }

  [TestMethod]
  public async Task Delete()
  {
    var result = await Repository.Delete(_savedCollection);

    Assert.IsTrue(result, "Result should be true");

    var exists = await Repository.Exists(_savedCollection.Name);

    Assert.IsFalse(exists, "Collection should not exists");
  }

  [TestMethod]
  public async Task Update()
  {
    _savedCollection.CollectionLists.Add(new("List 3", "Query 3", [MTGCardDTOMocker.Mock("Card")]));

    var result = await Repository.Update(_savedCollection);

    Assert.IsTrue(result, "Result should be true");

    var dbCollection = await Repository.Get(_savedCollection.Name);

    Assert.AreEqual(_savedCollection.CollectionLists.Count, dbCollection?.CollectionLists.Count);
  }
}
