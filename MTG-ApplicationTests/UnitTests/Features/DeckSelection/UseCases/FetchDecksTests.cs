using MTGApplication.Features.DeckSelection.Models;
using MTGApplication.Features.DeckSelection.UseCases;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Databases.Repositories.DeckRepository.Models;
using MTGApplicationTests.TestUtility.Database;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckSelection.UseCases;

[TestClass]
public class FetchDecksTests
{
  [TestMethod]
  public async Task Fetch_DecksReturned()
  {
    var decks = new MTGCardDeckDTO[]
    {
      new("Deck 1"),
      new("Deck 2"),
      new("Deck 3"),
    };
    var result = await new FetchDecks(new SimpleTestRepository<MTGCardDeckDTO>()
    {
      GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>(decks)
    }, new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success([])
    }).Execute();

    Assert.HasCount(3, result);
  }

  [TestMethod]
  public async Task Fetch_DecksOrdered_ByName()
  {
    var decks = new MTGCardDeckDTO[]
    {
      new("Deck 3"),
      new("Deck 1"),
      new("Deck 2"),
    };
    var result = await new FetchDecks(new SimpleTestRepository<MTGCardDeckDTO>()
    {
      GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>(decks)
    }, new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success([])
    }).Execute();

    CollectionAssert.AreEqual(
      expected: new DeckSelectionDeck[] {
        new() { Name = "Deck 1"},
        new() { Name = "Deck 2"},
        new() { Name = "Deck 3"},
      },
      actual: result.ToList());
  }

  [TestMethod]
  public async Task Fetch_ImageUri_Set()
  {
    var commanderId = Guid.NewGuid();
    var decks = new MTGCardDeckDTO[]
    {
      new(name: "Deck 1", commander: MTGCardDTOMocker.Mock(name: "Commander", scryfallId: commanderId)),
    };
    var result = await new FetchDecks(new SimpleTestRepository<MTGCardDeckDTO>()
    {
      GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>(decks)
    }, new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success([
        new(MTGCardInfoMocker.MockInfo(
          scryfallId: commanderId,
          frontFace: MTGCardInfoMocker.MockFace(imageUri: "Uri")))
        ])
    }).Execute();

    Assert.AreEqual("Uri", result.First().ImageUri);
  }

  [TestMethod]
  public async Task Fetch_Colors_Set()
  {
    var commanderId = Guid.NewGuid();
    var partnerId = Guid.NewGuid();
    var decks = new MTGCardDeckDTO[]
    {
      new(
        name: "Deck 1",
        commander: MTGCardDTOMocker.Mock(name: "Commander", scryfallId: commanderId),
        partner: MTGCardDTOMocker.Mock(name: "Partner", scryfallId: partnerId)),
    };
    var result = await new FetchDecks(new SimpleTestRepository<MTGCardDeckDTO>()
    {
      GetAllResult = () => Task.FromResult<IEnumerable<MTGCardDeckDTO>>(decks)
    }, new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success([
        new(MTGCardInfoMocker.MockInfo(
          scryfallId: commanderId,
          frontFace: MTGCardInfoMocker.MockFace(colors: [MTGCardInfo.ColorTypes.W, MTGCardInfo.ColorTypes.U ]))),
        new(MTGCardInfoMocker.MockInfo(
          scryfallId: partnerId,
          frontFace: MTGCardInfoMocker.MockFace(colors: [MTGCardInfo.ColorTypes.B ]))),
        ])
    }).Execute();

    CollectionAssert.AreEqual(
      expected: new MTGCardInfo.ColorTypes[] { MTGCardInfo.ColorTypes.W, MTGCardInfo.ColorTypes.U, MTGCardInfo.ColorTypes.B },
      actual: result.First().Colors);
  }
}
