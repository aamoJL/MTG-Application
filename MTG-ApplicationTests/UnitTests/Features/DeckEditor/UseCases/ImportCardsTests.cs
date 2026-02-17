using MTGApplication.Features.DeckEditor.UseCases;
using MTGApplication.General.Extensions;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Importers;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.UnitTests.Features.DeckEditor.UseCases;

[TestClass]
public class ImportCardsTests
{
  [TestMethod]
  public async Task Import_Internal()
  {
    var expected = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

    var importer = new TestMTGCardImporter();
    var scryfallImporter = new TestScryfallImporter();
    var edhImporter = new TestEdhrecImporter();

    JsonExtensions.TrySerializeObject(expected, out var serialized);

    Assert.IsNotNull(serialized);

    var result = await new ImportCards(importer, edhImporter, scryfallImporter).Execute(serialized);

    Assert.AreEqual(expected, result.Found.First());
  }

  [TestMethod]
  public async Task Import_EdhrecImage()
  {
    var expected = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

    var importer = new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success([expected])
    };
    var scryfallImporter = new TestScryfallImporter();
    var edhImporter = new TestEdhrecImporter()
    {
      ParseResult = "Card Name"
    };

    var result = await new ImportCards(importer, edhImporter, scryfallImporter).Execute("EDHREC Uri");

    Assert.AreEqual(expected, result.Found.First());
  }

  [TestMethod]
  public async Task Import_ScryfallImage_WithId()
  {
    var expected = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

    var importer = new TestMTGCardImporter();
    var scryfallImporter = new TestScryfallImporter()
    {
      Result = TestMTGCardImporter.Success([expected])
    };
    var edhImporter = new TestEdhrecImporter()
    {
      ParseResult = string.Empty
    };

    var result = await new ImportCards(importer, edhImporter, scryfallImporter)
      .Execute("https://cards.scryfall.io/large/front/3/a/3a63c06a-7c59-4b72-b916-e5b6ad78c684.jpg?1769006772");

    Assert.AreEqual(expected, result.Found.First());
  }

  [TestMethod]
  public async Task Import_ScryfallImage_WithName()
  {
    var expected = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

    var importer = new TestMTGCardImporter();
    var scryfallImporter = new TestScryfallImporter()
    {
      Result = TestMTGCardImporter.Success([expected])
    };
    var edhImporter = new TestEdhrecImporter()
    {
      ParseResult = string.Empty
    };

    var result = await new ImportCards(importer, edhImporter, scryfallImporter)
      .Execute("https://scryfall.com/card/ecc/57/sol-ring");

    Assert.AreEqual(expected, result.Found.First());
  }

  [TestMethod]
  public async Task Import_CardImporter()
  {
    var expected = new CardImportResult.Card(MTGCardInfoMocker.MockInfo());

    var importer = new TestMTGCardImporter()
    {
      Result = TestMTGCardImporter.Success([expected])
    };
    var scryfallImporter = new TestScryfallImporter();
    var edhImporter = new TestEdhrecImporter()
    {
      ParseResult = string.Empty
    };

    var result = await new ImportCards(importer, edhImporter, scryfallImporter)
      .Execute("Import text");

    Assert.AreEqual(expected, result.Found.First());
  }
}
