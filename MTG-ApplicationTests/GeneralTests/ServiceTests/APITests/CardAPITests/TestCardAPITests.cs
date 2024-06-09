using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.General.Models.Card;
using MTGApplicationTests.TestUtility.API;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.GeneralTests.Services.APITests.CardAPITests;

[TestClass]
public class TestCardAPITests
{
  [TestMethod]
  public async Task FetchCardsTest()
  {
    var api = new TestCardAPI();
    Assert.AreEqual(0, (await api.ImportCardsWithSearchQuery(string.Empty)).Found.Length);
    Assert.AreEqual(0, (await api.ImportCardsWithSearchQuery("params")).Found.Length);

    api.ExpectedCards =
    [
      MTGCardModelMocker.CreateMTGCardModel(),
    ];
    Assert.AreEqual(0, (await api.ImportCardsWithSearchQuery(string.Empty)).Found.Length);
    Assert.AreEqual(1, (await api.ImportCardsWithSearchQuery("params")).Found.Length);
  }

  [TestMethod]
  public async Task FetchFromDTOsTest()
  {
    var api = new TestCardAPI();
    var cards = new List<DeckEditorMTGCard>
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
    var cards = new List<DeckEditorMTGCard>
    {
      MTGCardModelMocker.CreateMTGCardModel(),
      MTGCardModelMocker.CreateMTGCardModel(),
      MTGCardModelMocker.CreateMTGCardModel(),
    };

    //No expected cards
    Assert.AreEqual(0, (await api.ImportFromString("")).Found.Length);

    // Expected cards
    api.ExpectedCards = [.. cards];
    api.NotFoundCount = 4;
    Assert.AreEqual(3, (await api.ImportFromString("")).Found.Length);
    Assert.AreEqual(4, (await api.ImportFromString("")).NotFoundCount);
  }
}
