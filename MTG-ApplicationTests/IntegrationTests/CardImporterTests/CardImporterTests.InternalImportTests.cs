﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTGApplication.Features.DeckEditor.Editor.Services;
using MTGApplication.General.Models;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using MTGApplicationTests.TestUtility.Mocker;

namespace MTGApplicationTests.IntegrationTests.CardImporterTests;
public partial class CardImporterTests
{
  [TestClass]
  public class InternalImportTests
  {
    [TestMethod]
    public async Task Import_Serialized_CardFound()
    {
      var importer = new DeckEditorCardImporter(new ScryfallAPI());
      var card = new CardImportResult<MTGCardInfo>.Card(MTGCardInfoMocker.MockInfo(), Count: 5);

      JsonService.TrySerializeObject(card, out var json);

      var result = await importer.Import(json);

      Assert.AreEqual(CardImportResult.ImportSource.Internal, result.Source);
      Assert.AreEqual((card.Info.Name, card.Count),
        (result.Found.First().Info.Name, result.Found.First().Count));
    }
  }
}
