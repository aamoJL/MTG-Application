using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor;

public class CardImporter
{
  public CardImporter(ICardAPI<MTGCard> cardAPI) => CardAPI = cardAPI;

  public ICardAPI<MTGCard> CardAPI { get; }

  public async Task<ICardAPI<MTGCard>.Result> Import(string data)
  {
    if (TryParseCardJSON(data) is MTGCard card)
    {
      // Imported from the app
      return new(new MTGCard[] { card }, 0, 1);
    }
    else if (Uri.TryCreate(data, UriKind.Absolute, out var uri) && uri.Host == "edhrec.com")
    {
      // Imported from EDHREC.com
      data = uri.Segments[^1]; // Get the last part of the url, which should be the card's name
    }

    return await CardAPI.FetchFromString(data);
  }

  private MTGCard TryParseCardJSON(string json)
  {
    try { return JsonSerializer.Deserialize<MTGCard>(json); }
    catch { return null; }
  }
}