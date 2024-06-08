using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.IOService;
using System.Threading.Tasks;

namespace MTGApplication.General.Models.Card;

public class CardImporter(ICardAPI<DeckEditorMTGCard> cardAPI)
{
  public ICardAPI<DeckEditorMTGCard> CardAPI { get; } = cardAPI;

  public async Task<CardImportResult> Import(string data)
  {
    if (JsonService.TryDeserializeJson<DeckEditorMTGCard>(data, out var card))
      return new([card], 0, 1, CardImportResult.ImportSource.Internal); // Imported from the app

    if (EdhrecAPI.TryParseCardNameFromEdhrecUri(data, out var name))
      return await CardAPI.FetchFromString(name); // Imported from EDHREC.com

    return await CardAPI.FetchFromString(data);
  }
}