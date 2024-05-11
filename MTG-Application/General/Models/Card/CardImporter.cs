using MTGApplication.General.Services.API.CardAPI;
using System.Threading.Tasks;

namespace MTGApplication.General.Models.Card;

public class CardImporter
{
  public CardImporter(ICardAPI<MTGCard> cardAPI) => CardAPI = cardAPI;

  public ICardAPI<MTGCard> CardAPI { get; }

  public async Task<ICardAPI<MTGCard>.Result> Import(string data)
  {
    if (MTGCardParser.TryParseJson(data, out var card))
      return new(new MTGCard[] { card }, 0, 1); // Imported from the app

    if (EdhrecParser.TryParseNameFromUri(data, out var name))
      return await CardAPI.FetchFromString(name); // Imported from EDHREC.com

    return await CardAPI.FetchFromString(data);
  }
}