using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.IOService;
using System;
using System.Threading.Tasks;

namespace MTGApplication.General.Models.Card;

// TODO: integration tests

public class CardImporter
{
  public CardImporter(ICardAPI<MTGCard> cardAPI) => CardAPI = cardAPI;

  public ICardAPI<MTGCard> CardAPI { get; }

  public async Task<CardImportResult> Import(string data)
  {
    if (JsonService.TryDeserializeJson<MTGCard>(data, out var card))
      return new(new MTGCard[] { card }, 0, 1, CardImportResult.ImportSource.Internal); // Imported from the app

    if (EdhrecAPI.TryParseCardNameFromEdhrecUri(data, out var name))
      return await CardAPI.FetchFromString(name); // Imported from EDHREC.com

    return await CardAPI.FetchFromString(data);
  }
}