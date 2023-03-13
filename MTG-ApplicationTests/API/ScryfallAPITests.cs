using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.API;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplicationTests.Services;
using System.Text.Json;
using static MTGApplication.API.ScryfallAPI;

namespace MTGApplicationTests.API
{
  [TestClass]
  public class ScryfallAPITests
  {
    [TestMethod]
    public async Task FetchCardsTest()
    {
      var api = new ScryfallAPI();
      string searchQuery = "asd";

      var fetchedCards = await api.FetchCardsWithParameters(searchQuery, 2);

      Assert.AreEqual(2, fetchedCards.Length);
      Assert.AreEqual(APIName, ICardAPI<MTGCard>.GetAPIName(fetchedCards[0]));
    }

    [TestMethod]
    public async Task FetchCardsTest_Empty()
    {
      var api = new ScryfallAPI();
      string searchQuery = "";

      var fetchedCards = await api.FetchCardsWithParameters(searchQuery);

      Assert.AreEqual(0, fetchedCards.Length);
    }

    [TestMethod]
    public async Task FetchFromStringTest()
    {
      var api = new ScryfallAPI();
      var importString = @"
        Black Lotus
        asd
        1 Blightstep Pathway
        4 Befriending the Moths // Imperial Moth
        _____ Bird Gets the Worm
        Katerina of Myra's Marvels
        Wear // Tear
        2 57e547cd-eba4-4a75-9c4e-8eeb6e00ddc4
        ";

      var (Found, NotFoundCount) = await api.FetchFromString(importString);
      Assert.IsNotNull(Found);
      Assert.AreEqual(7, Found.Length);
      Assert.AreEqual(11, Found.Sum(x => x.Count));
    }

    [TestMethod]
    public void FetchScryfallJsonObjectTest()
    {
      var api = new ScryfallAPI();
      string searchUri = api.GetSearchUri("asd");

      Assert.IsNotNull(FetchScryfallJsonObject(searchUri));
    }

    [TestMethod]
    public async Task FetchFromDTOsTest()
    {
      var api = new ScryfallAPI();
      var cards = new MTGCard[]
      {
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Against All Odds", scryfallId: Guid.Parse("3cd8dd4e-6892-49d7-8fae-97d04f9f6c84")),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Annex Sentry", scryfallId: Guid.Parse("04baad61-1b51-4602-9e33-0de4a9f34793")),
        Mocker.MTGCardModelMocker.CreateMTGCardModel(name: "Apostle of Invasion", scryfallId: Guid.Parse("8a973487-5def-4771-bb77-5748cbd2f469")),
      };

      var (Found, NotFoundCount) = await api.FetchFromDTOs(cards.Select(x => new CardDTO(x)).ToArray());

      Assert.AreEqual(cards[0].Info.Name, Found[0].Info.Name);
      Assert.AreEqual(cards[1].Info.Name, Found[1].Info.Name);
      Assert.AreEqual(cards[2].Info.Name, Found[2].Info.Name);
    }

    [TestMethod]
    public async Task FetchCardsFromPageTest_Empty()
    {
      var api = new ScryfallAPI();
      string pageUri = "";
      var (cards, nextPageUri, totalCount) = await api.FetchCardsFromPage(pageUri);

      Assert.AreEqual(0, totalCount);
      Assert.AreEqual(0, cards.Length);
      Assert.AreEqual(string.Empty, nextPageUri);
    }

    [TestMethod]
    public async Task FetchCardsFromPageTest()
    {
      var api = new ScryfallAPI();
      string pageUri = 
        "https://api.scryfall.com/cards/search?dir=asc&format=json&include_extras=false&include_multilingual=false&include_variations=false&order=released&page=2&q=set%3Aneo+unique%3Acards+order%3Areleased+direction%3Aasc+format%3Aany+game%3Apaper&unique=cards";
      var (cards, nextPageUri, totalCount) = await api.FetchCardsFromPage(pageUri);

      Assert.IsTrue(totalCount > 0);
      Assert.IsTrue(cards.Length > 0);
      Assert.AreEqual(string.Empty, nextPageUri);
    }

    [TestMethod]
    public void GetSearchUriTest_Empty()
    {
      var api = new ScryfallAPI();
      Assert.AreEqual(string.Empty, api.GetSearchUri(""));
    }

    [TestMethod]
    public async Task FetchCardsFromUriTest()
    {
      var api = new ScryfallAPI();
      var uri = 
        "https://api.scryfall.com/cards/search?order=released&q=oracleid%3A48e603a2-b965-4fbc-ad57-4388bce5ac8b&unique=prints";

      var cards = await api.FetchCardsFromUri(uri);
      Assert.IsTrue(cards.Length > 0);
    }

    [TestMethod]
    public async Task FetchCardsFromUriTest_Empty()
    {
      var api = new ScryfallAPI();
      var uri = string.Empty;

      var cards = await api.FetchCardsFromUri(uri);
      Assert.AreEqual(0, cards.Length);
    }

    #region Identifier

    [TestMethod]
    public void ScryfallIdentifier_ToObjectTest_ID()
    {
      var sID = Guid.NewGuid();
      var IllID = Guid.NewGuid();
      var identifier = new ScryfallIdentifier()
      {
        ScryfallId = sID,
        CardCount = 5,
        Name = "Test",
        SetCode = "tst",
        IllustrationId = IllID,
        PreferedSchema = ScryfallIdentifier.IdentifierSchema.ID
      };

      var expected = JsonSerializer.Serialize(new { id = sID });
      var result = JsonSerializer.Serialize(identifier.ToObject());

      Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ScryfallIdentifier_ToObjectTest_ILLUSTRATION_ID()
    {
      var sID = Guid.NewGuid();
      var IllID = Guid.NewGuid();
      var identifier = new ScryfallIdentifier()
      {
        ScryfallId = sID,
        CardCount = 5,
        Name = "Test",
        SetCode = "tst",
        IllustrationId = IllID,
        PreferedSchema = ScryfallIdentifier.IdentifierSchema.ILLUSTRATION_ID
      };

      var expected = JsonSerializer.Serialize(new { illustration_id = IllID });
      var result = JsonSerializer.Serialize(identifier.ToObject());

      Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ScryfallIdentifier_ToObjectTest_SET_NAME()
    {
      var sID = Guid.NewGuid();
      var IllID = Guid.NewGuid();
      var identifier = new ScryfallIdentifier()
      {
        ScryfallId = sID,
        CardCount = 5,
        Name = "Test",
        SetCode = "tst",
        IllustrationId = IllID,
        PreferedSchema = ScryfallIdentifier.IdentifierSchema.NAME_SET
      };

      var expected = JsonSerializer.Serialize(new { name = identifier.Name, set = identifier.SetCode });
      var result = JsonSerializer.Serialize(identifier.ToObject());

      Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void ScryfallIdentifier_ToObjectTest_NAME()
    {
      var sID = Guid.NewGuid();
      var IllID = Guid.NewGuid();
      var identifier = new ScryfallIdentifier()
      {
        ScryfallId = sID,
        CardCount = 5,
        Name = "Test",
        SetCode = "tst",
        IllustrationId = IllID,
        PreferedSchema = ScryfallIdentifier.IdentifierSchema.NAME
      };

      var expected = JsonSerializer.Serialize(new { name = identifier.Name });
      var result = JsonSerializer.Serialize(identifier.ToObject());

      Assert.AreEqual(expected, result);
    }
    
    #endregion
  }
}
