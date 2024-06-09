using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.IOService;
using System.Threading.Tasks;

namespace MTGApplication.General.Models.Card;

public class DeckEditorCardImporter(MTGCardImporter importer)
{
  public async Task<CardImportResult<MTGCardInfo>> Import(string data)
  {
    if (JsonService.TryDeserializeJson<DeckEditorMTGCard>(data, out var card))
      return new([new CardImportResult<MTGCardInfo>.Card(card.Info, card.Count)], 0, 1, CardImportResult.ImportSource.Internal); // Imported from the app

    if (EdhrecImporter.TryParseCardNameFromEdhrecUri(data, out var name))
      return await importer.ImportFromString(name); // Imported from EDHREC.com

    return await importer.ImportFromString(data);
  }
}