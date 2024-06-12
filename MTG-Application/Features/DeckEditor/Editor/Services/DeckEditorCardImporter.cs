using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.IOServices;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.Services;

public class DeckEditorCardImporter(MTGCardImporter importer)
{
  public async Task<CardImportResult> Import(string data)
  {
    if (JsonService.TryDeserializeJson<CardImportResult.Card>(data, out var card))
      return new([new CardImportResult.Card(card.Info, card.Count)], 0, 1, CardImportResult.ImportSource.Internal); // Imported from the app

    if (EdhrecImporter.TryParseCardNameFromEdhrecUri(data, out var name))
      return await importer.ImportFromString(name); // Imported from EDHREC.com

    return await importer.ImportFromString(data);
  }
}