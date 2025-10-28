using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Importers.CardImporter;

namespace MTGApplicationTests.IntegrationTests.CardImporterTests;
public partial class CardImporterTests
{
  [TestClass]
  public class EdhrecUriImportTests
  {
    [TestMethod]
    public async Task Import_WithValidUri_CardFound()
    {
      var importer = new DeckEditorCardImporter(new ScryfallAPI());
      var uri = "https://edhrec.com/cards/sol-ring";

      var result = await importer.Import(uri);

      Assert.AreEqual(CardImportResult.ImportSource.External, result.Source);
      Assert.AreEqual("Sol Ring", result.Found.First().Info.Name);
    }

    [TestMethod]
    public async Task Import_WithInvalidUri_NoCardFound()
    {
      var importer = new DeckEditorCardImporter(new ScryfallAPI());
      var uri = "https://edhrec.com/cards/xxxxx";

      var result = await importer.Import(uri);

      Assert.AreEqual(CardImportResult.ImportSource.External, result.Source);
      Assert.AreEqual(1, result.NotFoundCount);
    }
  }
}
