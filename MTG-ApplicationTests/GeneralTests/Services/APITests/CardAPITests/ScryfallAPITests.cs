using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplicationTests.TestUtility.API;

namespace MTGApplicationTests.GeneralTests.Services.APITests.CardAPITests;

[TestClass]
public class ScryfallAPITests
{
  [TestMethod("API should convert json file into cards")]
  public async Task JsonResponse_ConversionToCard_ReturnCard()
  {
    var result = await new TestScryfallAPI().GetCardsFromSampleJSON();

    Assert.IsTrue(result.Length > 0, "Result should have cards");
  }
}

// TODO: move to integration testing

//[TestClass]
//public class ScryfallAPITests
//{
//  [TestMethod]
//  public async Task FetchCardsTest()
//  {
//    var api = new ScryfallAPI();
//    var searchQuery = "asd";

//    var result = await api.FetchCardsWithSearchQuery(searchQuery);

//    Assert.IsTrue(result.Found.Length > 0);
//    Assert.AreEqual(api.Name, result.Found[0].APIName);
//  }

//  [TestMethod]
//  public async Task FetchCardsTest_Empty()
//  {
//    var api = new ScryfallAPI();
//    var searchQuery = "";

//    var result = await api.FetchCardsWithSearchQuery(searchQuery);

//    Assert.AreEqual(0, result.Found.Length);
//  }

//  [TestMethod]
//  public async Task FetchCardsTest_ReversibleCards()
//  {
//    var api = new ScryfallAPI();
//    var searchQuery = @"is:reversible";

//    var result = await api.FetchCardsWithSearchQuery(searchQuery);
//    Assert.IsTrue(result.Found.Length > 0);
//  }

//  [TestMethod]
//  public async Task FetchFromStringTest()
//  {
//    var api = new ScryfallAPI();
//    var importString = @"
//        Black Lotus
//        asd
//        1 Blightstep Pathway
//        4 Befriending the Moths // Imperial Moth
//        _____ Bird Gets the Worm
//        Katerina of Myra's Marvels
//        Wear // Tear
//        2 57e547cd-eba4-4a75-9c4e-8eeb6e00ddc4
//        ";

//    var result = await api.FetchFromString(importString);
//    Assert.IsNotNull(result.Found);
//    Assert.AreEqual(7, result.Found.Length);
//    Assert.AreEqual(11, result.Found.Sum(x => x.Count));
//  }

//  [TestMethod]
//  public async Task FetchFromDTOsTest()
//  {
//    var api = new ScryfallAPI();
//    var cards = new MTGCard[]
//    {
//      MTGCardModelMocker.CreateMTGCardModel(name: "Against All Odds", scryfallId: Guid.Parse("3cd8dd4e-6892-49d7-8fae-97d04f9f6c84")),
//      MTGCardModelMocker.CreateMTGCardModel(name: "Annex Sentry", scryfallId: Guid.Parse("04baad61-1b51-4602-9e33-0de4a9f34793")),
//      MTGCardModelMocker.CreateMTGCardModel(name: "Apostle of Invasion", scryfallId: Guid.Parse("8a973487-5def-4771-bb77-5748cbd2f469")),
//    };

//    var result = await api.FetchFromDTOs(cards.Select(x => new MTGCardDTO(x)).ToArray());

//    Assert.AreEqual(cards[0].Info.Name, result.Found[0].Info.Name);
//    Assert.AreEqual(cards[1].Info.Name, result.Found[1].Info.Name);
//    Assert.AreEqual(cards[2].Info.Name, result.Found[2].Info.Name);
//  }

//  [TestMethod]
//  public async Task FetchCardsFromPageTest_Empty()
//  {
//    var api = new ScryfallAPI();
//    var pageUri = "";
//    var result = await api.FetchFromUri(pageUri);

//    Assert.AreEqual(0, result.TotalCount);
//    Assert.AreEqual(0, result.Found.Length);
//    Assert.AreEqual(string.Empty, result.NextPageUri);
//  }

//  [TestMethod]
//  public async Task FetchCardsFromPageTest()
//  {
//    var api = new ScryfallAPI();
//    var pageUri =
//      "https://api.scryfall.com/cards/search?dir=asc&format=json&include_extras=false&include_multilingual=false&include_variations=false&order=released&page=2&q=set%3Aneo+unique%3Acards+order%3Areleased+direction%3Aasc+format%3Aany+game%3Apaper&unique=cards";
//    var result = await api.FetchFromUri(pageUri);

//    Assert.IsTrue(result.TotalCount > 0);
//    Assert.IsTrue(result.Found.Length > 0);
//    Assert.AreEqual(string.Empty, result.NextPageUri);
//  }

//  [TestMethod]
//  public void GetSearchUriTest_Empty()
//  {
//    var api = new ScryfallAPI();
//    Assert.AreEqual(string.Empty, api.GetSearchUri(""));
//  }

//  [TestMethod]
//  public async Task FetchCardsFromUriTest()
//  {
//    var api = new ScryfallAPI();
//    var uri =
//      "https://api.scryfall.com/cards/search?order=released&q=oracleid%3A48e603a2-b965-4fbc-ad57-4388bce5ac8b&unique=prints";

//    var result = await api.FetchFromUri(uri);
//    Assert.IsTrue(result.Found.Length > 0);
//  }

//  [TestMethod]
//  public async Task FetchCardsFromUriTest_Empty()
//  {
//    var api = new ScryfallAPI();
//    var uri = string.Empty;

//    var result = await api.FetchFromUri(uri);
//    Assert.AreEqual(0, result.Found.Length);
//  }

//}
