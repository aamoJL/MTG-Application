using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.General.Models.Card.CardImportResult;

namespace MTGApplicationTests.TestUtility.API;

// TODO: clean

public class TestCardAPI(MTGCard[]? expectedCards = null, int notFoundCount = 0) : ICardAPI<MTGCard>
{
  public MTGCard[]? ExpectedCards { get; set; } = expectedCards;
  public int NotFoundCount { get; set; } = notFoundCount;

  public int PageSize => 40;
  public string Name => "Test Card API";

  public async Task<CardImportResult> FetchCardsWithSearchQuery(string searchParams)
  {
    if (string.IsNullOrEmpty(searchParams)) { return Empty(); }
    return await Task.Run(() => ExpectedCards != null ? new CardImportResult(ExpectedCards, NotFoundCount, ExpectedCards!.Length, ImportSource.External) : Empty());
  }

  public async Task<CardImportResult> FetchFromDTOs(CardDTO[] dtoArray)
  {
    var cards = dtoArray.Select(x => MTGCardModelMocker.FromDTO((MTGCardDTO)x)).ToArray();

    if (ExpectedCards == null) { return await Task.Run(() => new CardImportResult(cards, 0, cards.Length, ImportSource.External)); }
    else
    {
      var found = ExpectedCards!.Where(ex => cards.FirstOrDefault(x => x.Info.ScryfallId == ex.Info.ScryfallId) != null)?.ToList() ?? new List<MTGCard>();
      var notFoundCount = ExpectedCards!.Length - found.Count;

      return await Task.Run(() => new CardImportResult([.. found], notFoundCount, found.Count, ImportSource.External));
    }
  }

  public async Task<CardImportResult> FetchFromString(string importText)
    => await Task.Run(() => ExpectedCards != null ? new CardImportResult(ExpectedCards, NotFoundCount, ExpectedCards!.Length, ImportSource.External) : Empty());

  public async Task<CardImportResult> FetchFromUri(string pageUri, bool paperOnly = false, bool fetchAll = false)
  {
    var cards = string.IsNullOrEmpty(pageUri) ? [] : ExpectedCards ?? [];
    return await Task.Run(() => new CardImportResult(cards, NotFoundCount, cards.Length, ImportSource.External));
  }

  public string GetSearchUri(string searchParams) => searchParams;
}

[TestClass]
public class TestCardAPITests
{
  [TestMethod]
  public async Task FetchCardsTest()
  {
    var api = new TestCardAPI();
    Assert.AreEqual(0, (await api.FetchCardsWithSearchQuery(string.Empty)).Found.Length);
    Assert.AreEqual(0, (await api.FetchCardsWithSearchQuery("params")).Found.Length);

    api.ExpectedCards =
    [
      MTGCardModelMocker.CreateMTGCardModel(),
    ];
    Assert.AreEqual(0, (await api.FetchCardsWithSearchQuery(string.Empty)).Found.Length);
    Assert.AreEqual(1, (await api.FetchCardsWithSearchQuery("params")).Found.Length);
  }

  [TestMethod]
  public async Task FetchFromDTOsTest()
  {
    var api = new TestCardAPI();
    var cards = new List<MTGCard>
    {
      MTGCardModelMocker.CreateMTGCardModel(),
      MTGCardModelMocker.CreateMTGCardModel(),
      MTGCardModelMocker.CreateMTGCardModel(),
    };
    //No expected cards
    var dtos = cards.Select(x => new MTGCardDTO(x)).ToArray();
    Assert.AreEqual(3, (await api.FetchFromDTOs(dtos)).Found.Length);

    // Expected cards are same as DTOs
    api.ExpectedCards = [.. cards];
    Assert.AreEqual(3, (await api.FetchFromDTOs(dtos)).Found.Length);
    Assert.AreEqual(0, (await api.FetchFromDTOs(dtos)).NotFoundCount);

    // Expected cards has one more card than DTOs
    cards.Add(MTGCardModelMocker.CreateMTGCardModel());
    api.ExpectedCards = [.. cards];
    Assert.AreEqual(3, (await api.FetchFromDTOs(dtos)).Found.Length);
    Assert.AreEqual(1, (await api.FetchFromDTOs(dtos)).NotFoundCount);
  }

  [TestMethod]
  public async Task FetchFromStringTest()
  {
    var api = new TestCardAPI();
    var cards = new List<MTGCard>
    {
      MTGCardModelMocker.CreateMTGCardModel(),
      MTGCardModelMocker.CreateMTGCardModel(),
      MTGCardModelMocker.CreateMTGCardModel(),
    };

    //No expected cards
    Assert.AreEqual(0, (await api.FetchFromString("")).Found.Length);

    // Expected cards
    api.ExpectedCards = [.. cards];
    api.NotFoundCount = 4;
    Assert.AreEqual(3, (await api.FetchFromString("")).Found.Length);
    Assert.AreEqual(4, (await api.FetchFromString("")).NotFoundCount);
  }
}
