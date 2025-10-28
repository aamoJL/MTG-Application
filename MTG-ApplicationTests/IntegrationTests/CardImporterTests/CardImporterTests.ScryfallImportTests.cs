using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Importers.CardImporter;

namespace MTGApplicationTests.IntegrationTests.CardImporterTests;
public partial class CardImporterTests
{
  [TestClass]
  public class ScryfallImportTests
  {
    [TestMethod]
    public async Task Import_WithValidIds_CardsFound()
    {
      var importer = new DeckEditorCardImporter(new ScryfallAPI());
      var idListString = string.Join(Environment.NewLine,
      [
        "d9b1ed43-ee6c-43a2-ba94-5bf71c63e070",
        "8d2c4063-7322-4041-a870-b95598a03e29",
        "e4b8946b-6604-49ca-98db-18a540b1b4e5",
        "8ad44884-ae0d-40ae-87a9-bad043d4e9ad"
      ]);

      var result = await importer.Import(idListString);

      Assert.AreEqual(CardImportResult.ImportSource.External, result.Source);
      Assert.AreEqual(idListString, string.Join(Environment.NewLine,
        result.Found.Select(x => x.Info.ScryfallId)));
    }

    [TestMethod]
    public async Task Import_WithInvalidIds_NoCardsFound()
    {
      var importer = new DeckEditorCardImporter(new ScryfallAPI());
      var idListString = string.Join(Environment.NewLine,
      [
        "xxxxxxx-ee6c-43a2-ba94-5bf71c63e070",
        "xxxxxxx-7322-4041-a870-b95598a03e29",
        "xxxxxxx-6604-49ca-98db-18a540b1b4e5",
        "xxxxxxx-ae0d-40ae-87a9-bad043d4e9ad"
      ]);

      var result = await importer.Import(idListString);

      Assert.AreEqual(CardImportResult.ImportSource.External, result.Source);
      Assert.AreEqual(4, result.NotFoundCount);
    }

    [TestMethod]
    public async Task Import_WithIdUri_CardFound()
    {
      var importer = new DeckEditorCardImporter(new ScryfallAPI());
      var uri = "https://cards.scryfall.io/large/front/8/0/80fc51aa-64ca-4236-8cdb-670533b75f59.jpg?1736467426";

      var result = await importer.Import(uri);

      Assert.AreEqual(CardImportResult.ImportSource.External, result.Source);
      Assert.HasCount(1, result.Found);
      Assert.AreEqual(new Guid("80fc51aa-64ca-4236-8cdb-670533b75f59"), result.Found[0].Info.ScryfallId);
    }

    [TestMethod]
    public async Task Import_WithNameUri_CardFound()
    {
      var importer = new DeckEditorCardImporter(new ScryfallAPI());
      var uri = "https://scryfall.com/card/inr/2/decimator-of-the-provinces";

      var result = await importer.Import(uri);

      Assert.AreEqual(CardImportResult.ImportSource.External, result.Source);
      Assert.HasCount(1, result.Found);
      Assert.AreEqual("Decimator of the Provinces", result.Found[0].Info.Name);
    }

    [TestMethod]
    public async Task Import_WithInvalidUri_CardNotFound()
    {
      var importer = new DeckEditorCardImporter(new ScryfallAPI());
      var uri = "https://scryfall.com/xxxxxx/inr/2/xxxxxxxx-xx";

      var result = await importer.Import(uri);

      Assert.AreEqual(CardImportResult.ImportSource.External, result.Source);
      Assert.AreEqual(1, result.NotFoundCount);
      Assert.IsEmpty(result.Found);
    }
  }
}
