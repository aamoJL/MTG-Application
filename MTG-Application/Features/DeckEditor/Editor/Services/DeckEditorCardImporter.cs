using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.Services.IOServices;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.Services;

public class DeckEditorCardImporter(MTGCardImporter importer)
{
  /// <exception cref="System.InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="System.UriFormatException"></exception>
  public async Task<CardImportResult> Import(string data)
  {
    try
    {
      if (JsonService.TryDeserializeJson<CardImportResult.Card>(data, out var card))
        return new([new CardImportResult.Card(card.Info, card.Count)], 0, 1, CardImportResult.ImportSource.Internal); // Imported from the app

      if (EdhrecImporter.TryParseCardNameFromEdhrecUri(data, out var name))
        return await new FetchCardsWithImportString(importer).Execute(name); // Imported from EDHREC.com

      return await new FetchCardsWithImportString(importer).Execute(data);
    }
    catch { throw; }
  }
}