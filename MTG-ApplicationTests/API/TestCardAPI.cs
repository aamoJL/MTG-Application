using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplicationTests.Services;

namespace MTGApplicationTests.API
{
  public class TestCardAPI : ICardAPI<MTGCard>
  {
    public TestCardAPI(MTGCard[]? expectedCards = null, int notFoundCount = 0)
    {
      ExpectedCards = expectedCards;
      NotFoundCount = notFoundCount;
    }

    public MTGCard[]? ExpectedCards { get; set; }
    public int NotFoundCount { get; set; }

    public async Task<MTGCard[]> FetchCardsFromUri(string uri, int countLimit = int.MaxValue)
    {
      return await FetchCardsWithParameters(uri, countLimit);
    }
    public async Task<MTGCard[]> FetchCardsWithParameters(string searchParams, int countLimit = 700)
    {
      if (string.IsNullOrEmpty(searchParams)) { return Array.Empty<MTGCard>(); }
      return await Task.Run(() => ExpectedCards ?? Array.Empty<MTGCard>());
    }
    public async Task<(MTGCard[] Found, int NotFoundCount)> FetchFromDTOs(CardDTO[] dtoArray)
    {
      var cards = dtoArray.Select(x => Mocker.MTGCardModelMocker.FromDTO(x)).ToArray();

      if(ExpectedCards == null) { return await Task.Run(() => (cards, 0)); }
      else
      {
        var found = cards.Select(dtoCard => ExpectedCards.FirstOrDefault(ex => ex.Info.ScryfallId == dtoCard.Info.ScryfallId))?.ToList() ?? new List<MTGCard?>();
        var notFoundCount = ExpectedCards.Length - found.Count;
        return await Task.Run(() => (found.ToArray(), notFoundCount));
      }
    }
    public async Task<(MTGCard[] Found, int NotFoundCount)> FetchFromString(string importText)
    {
      return await Task.Run(() => (ExpectedCards ?? Array.Empty<MTGCard>(), NotFoundCount));
    }
  }

  [TestClass]
  public class TestCardAPITests
  {
    [TestMethod]
    public async Task FetchCardsTest()
    {
      var api = new TestCardAPI();
      Assert.AreEqual(0, (await api.FetchCardsWithParameters(string.Empty)).Length);
      Assert.AreEqual(0, (await api.FetchCardsWithParameters("params")).Length);
      Assert.AreEqual(0, (await api.FetchCardsFromUri(string.Empty)).Length);
      Assert.AreEqual(0, (await api.FetchCardsFromUri("params")).Length);

      api.ExpectedCards = new MTGCard[]
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(),
      };
      Assert.AreEqual(0, (await api.FetchCardsWithParameters(string.Empty)).Length);
      Assert.AreEqual(1, (await api.FetchCardsWithParameters("params")).Length);
      Assert.AreEqual(0, (await api.FetchCardsFromUri(string.Empty)).Length);
      Assert.AreEqual(1, (await api.FetchCardsFromUri("params")).Length);
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
      var dtos = cards.Select(x => new CardDTO(x)).ToArray();
      Assert.AreEqual(3, (await api.FetchFromDTOs(dtos)).Found.Length);

      // Expected cards are same as DTOs
      api.ExpectedCards = cards.ToArray();
      Assert.AreEqual(3, (await api.FetchFromDTOs(dtos)).Found.Length);
      Assert.AreEqual(0, (await api.FetchFromDTOs(dtos)).NotFoundCount);

      // Expected cards has one more card than DTOs
      cards.Add(Mocker.MTGCardModelMocker.CreateMTGCardModel());
      api.ExpectedCards = cards.ToArray();
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
      api.ExpectedCards = cards.ToArray();
      api.NotFoundCount = 4;
      Assert.AreEqual(3, (await api.FetchFromString("")).Found.Length);
      Assert.AreEqual(4, (await api.FetchFromString("")).NotFoundCount);
    }
  }
}
