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

    if (TryParseNameFromEdhrecUri(data, out var name))
      return await CardAPI.FetchFromString(name); // Imported from EDHREC.com

    return await CardAPI.FetchFromString(data);
  }

  /// <summary>
  /// Tries to parse a card <paramref name="name"/> from the given <paramref name="data"/>
  /// The <paramref name="data"/> should be an EDHREC card uri.
  /// </summary>
  /// <param name="data">EDHREC card Uri</param>
  /// <param name="name">Parsed card name</param>
  /// <returns><see langword="true"/> if the <paramref name="data"/> was successfully parsed; otherwise, <see langword="false"/></returns>
  protected static bool TryParseNameFromEdhrecUri(string data, out string name)
  {
    if (Uri.TryCreate(data, UriKind.Absolute, out var uri) && uri.Host == "edhrec.com")
      name = uri.Segments[^1]; // Name is the last segment of the Uri
    else
      name = default;

    return name != default;
  }
}