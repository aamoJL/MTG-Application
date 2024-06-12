using Microsoft.VisualStudio.TestTools.UnitTesting;
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
  }
}
