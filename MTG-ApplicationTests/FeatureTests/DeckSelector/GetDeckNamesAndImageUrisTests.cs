using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.API.CardAPI;
using MTGApplication.Features.CardDeck;
using MTGApplication.General.Databases.Repositories;
using MTGApplication.General.Models.Card;
using MTGApplication.Models.DTOs;
using MTGApplicationTests.Database;
using MTGApplicationTests.TestUtility;

namespace MTGApplicationTests.Features.DeckSelector;
[TestClass]
public class GetDeckNamesAndImageUrisTests
{
  private readonly TestCardDbContextFactory _ctxFactory = new();
  private readonly IRepository<MTGCardDeckDTO> _repository;
  private readonly ICardAPI<MTGCard> _cardAPI = new ScryfallAPI(); // mock

  public GetDeckNamesAndImageUrisTests() => _repository = new TestSQLiteMTGDeckRepository(_ctxFactory);

  [TestMethod("Should return names and image URIs for the repository decks")]
  public async Task Execute_HasDecks_ReturnDeckNamesAndImageUris()
  {
    _ctxFactory.Populate(new List<MTGCardDeckDTO>
      {
        new(name: "Deck 1", commander: new(scryfallId: new("4f8dc511-e307-4412-bb79-375a6077312d"), oracleId: new("8095ca78-db19-4724-a6ff-eacc85fa2274"), setCode: "otj", collectorNumber: "1")),
        new(name: "Deck 2", commander: new(scryfallId: new("4f8dc511-e307-4412-bb79-375a6077312d"), oracleId: new("8095ca78-db19-4724-a6ff-eacc85fa2274"), setCode: "otj", collectorNumber: "1")),
        new(name: "Deck 3")
      });

    var result = await new GetDeckNamesAndImageUris(new TestSQLiteMTGDeckRepository(_ctxFactory), _cardAPI).Execute();

    Assert.AreEqual(3, result.Count());
    Assert.AreEqual(2, result.Where(x => !string.IsNullOrEmpty(x.ImageUri)).Count());
  }

  [TestMethod("Should return an empty list if the repository has no decks")]
  public async Task Execute_HasNoDecks_ReturnEmptyList()
  {
    var result = await new GetDeckNamesAndImageUris(_repository, _cardAPI).Execute();

    Assert.IsFalse(result.Any(), "Result should be empty");
  }
}
