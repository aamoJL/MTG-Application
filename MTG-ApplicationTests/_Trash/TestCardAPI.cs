﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplicationTests.Services;
using static MTGApplication.General.Services.API.CardAPI.ICardAPI<MTGApplication.General.Models.Card.MTGCard>;

namespace MTGApplicationTests.API;

public class TestCardAPI(MTGCard[]? expectedCards = null, int notFoundCount = 0) : ICardAPI<MTGCard>
{
  public MTGCard[]? ExpectedCards { get; set; } = expectedCards;
  public int NotFoundCount { get; set; } = notFoundCount;

  public int PageSize => 40;
  public string Name => "Test Card API";

  public async Task<Result> FetchCardsWithSearchQuery(string searchParams)
  {
    if (string.IsNullOrEmpty(searchParams)) { return Result.Empty(); }
    return await Task.Run(() => ExpectedCards != null ? new Result(ExpectedCards, NotFoundCount, ExpectedCards!.Length) : Result.Empty());
  }
  
  public async Task<Result> FetchFromDTOs(CardDTO[] dtoArray)
  {
    var cards = dtoArray.Select(x => Mocker.MTGCardModelMocker.FromDTO((MTGCardDTO)x)).ToArray();

    if (ExpectedCards == null) { return await Task.Run(() => new Result(cards, 0, cards.Length)); }
    else
    {
      var found = ExpectedCards!.Where(ex => cards.FirstOrDefault(x => x.Info.ScryfallId == ex.Info.ScryfallId) != null)?.ToList() ?? new List<MTGCard>();
      var notFoundCount = ExpectedCards!.Length - found.Count;

      return await Task.Run(() => new Result([.. found], notFoundCount, found.Count));
    }
  }
  
  public async Task<Result> FetchFromString(string importText)
  {
    return await Task.Run(() => ExpectedCards != null ? new Result(ExpectedCards, NotFoundCount, ExpectedCards!.Length) : Result.Empty());
  }
  
  public async Task<Result> FetchFromUri(string pageUri, bool paperOnly = false)
  {
    var cards = string.IsNullOrEmpty(pageUri) ? [] : ExpectedCards ?? [];
    return await Task.Run(() => new Result(cards, NotFoundCount, cards.Length));
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
      Mocker.MTGCardModelMocker.CreateMTGCardModel(),
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
      Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(),
    };
    //No expected cards
    var dtos = cards.Select(x => new MTGCardDTO(x)).ToArray();
    Assert.AreEqual(3, (await api.FetchFromDTOs(dtos)).Found.Length);

    // Expected cards are same as DTOs
    api.ExpectedCards = [.. cards];
    Assert.AreEqual(3, (await api.FetchFromDTOs(dtos)).Found.Length);
    Assert.AreEqual(0, (await api.FetchFromDTOs(dtos)).NotFoundCount);

    // Expected cards has one more card than DTOs
    cards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
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
      Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      Mocker.MTGCardModelMocker.CreateMTGCardModel(),
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
