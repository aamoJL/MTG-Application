using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.API;
using MTGApplication.Models;
using MTGApplicationTests.Services;
using System.Text.Json;
using static MTGApplication.API.ScryfallAPI;

namespace MTGApplicationTests.API;

[TestClass]
public class ScryfallAPITests
{
  [TestMethod]
  public async Task FetchCardsTest()
  {
    var api = new ScryfallAPI();
    var searchQuery = "asd";

    var result = await api.FetchCardsWithParameters(searchQuery);

    Assert.IsTrue(result.Found.Length > 0);
    Assert.AreEqual(APIName, result.Found[0].APIName);
  }

  [TestMethod]
  public async Task FetchCardsTest_Empty()
  {
    var api = new ScryfallAPI();
    var searchQuery = "";

    var result = await api.FetchCardsWithParameters(searchQuery);

    Assert.AreEqual(0, result.Found.Length);
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

    var result = await api.FetchFromString(importString);
    Assert.IsNotNull(result.Found);
    Assert.AreEqual(7, result.Found.Length);
    Assert.AreEqual(11, result.Found.Sum(x => x.Count));
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

    var result = await api.FetchFromDTOs(cards.Select(x => new MTGCardDTO(x)).ToArray());

    Assert.AreEqual(cards[0].Info.Name, result.Found[0].Info.Name);
    Assert.AreEqual(cards[1].Info.Name, result.Found[1].Info.Name);
    Assert.AreEqual(cards[2].Info.Name, result.Found[2].Info.Name);
  }

  [TestMethod]
  public async Task FetchCardsFromPageTest_Empty()
  {
    var api = new ScryfallAPI();
    var pageUri = "";
    var result = await api.FetchFromUri(pageUri);

    Assert.AreEqual(0, result.TotalCount);
    Assert.AreEqual(0, result.Found.Length);
    Assert.AreEqual(string.Empty, result.NextPageUri);
  }

  [TestMethod]
  public async Task FetchCardsFromPageTest()
  {
    var api = new ScryfallAPI();
    var pageUri =
      "https://api.scryfall.com/cards/search?dir=asc&format=json&include_extras=false&include_multilingual=false&include_variations=false&order=released&page=2&q=set%3Aneo+unique%3Acards+order%3Areleased+direction%3Aasc+format%3Aany+game%3Apaper&unique=cards";
    var result = await api.FetchFromUri(pageUri);

    Assert.IsTrue(result.TotalCount > 0);
    Assert.IsTrue(result.Found.Length > 0);
    Assert.AreEqual(string.Empty, result.NextPageUri);
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

    var result = await api.FetchFromUri(uri);
    Assert.IsTrue(result.Found.Length > 0);
  }

  [TestMethod]
  public async Task FetchCardsFromUriTest_Empty()
  {
    var api = new ScryfallAPI();
    var uri = string.Empty;

    var result = await api.FetchFromUri(uri);
    Assert.AreEqual(0, result.Found.Length);
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
