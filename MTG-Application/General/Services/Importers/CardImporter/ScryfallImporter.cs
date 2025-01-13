using MTGApplication.General.Services.API.CardAPI;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter;

public partial class ScryfallImporter
{
  private static readonly string NAME_HOST = "scryfall.com";
  private static readonly string IMAGE_HOST = $"cards.scryfall.io";

  public static async Task<CardImportResult?> TryImportFromUri(string data)
  {
    var result = CardImportResult.Empty();

    if (TryParseCardIdFromUri(data) is Guid id)
      result = await new ScryfallAPI().ImportWithId(id);
    else if (TryParseCardNameFromUri(data) is string name)
      result = await new ScryfallAPI().ImportWithName(name, fuzzy: true);

    return result.TotalCount > 0 ? result : null;
  }

  // Example id uri: https://cards.scryfall.io/large/front/8/0/80fc51aa-64ca-4236-8cdb-670533b75f59.jpg?1736467426
  private static Guid? TryParseCardIdFromUri(string data)
  {
    return (Uri.TryCreate(data, UriKind.Absolute, out var uri)
      && uri.Host == IMAGE_HOST
      && uri.Segments.LastOrDefault() is string imageFileName
      && Path.GetFileNameWithoutExtension(imageFileName) is string idString
      && Guid.TryParse(idString, out var id))
      ? id : null;
  }

  // Example name uri: https://scryfall.com/card/inr/2/decimator-of-the-provinces
  private static string? TryParseCardNameFromUri(string data)
  {
    return (Uri.TryCreate(data, UriKind.Absolute, out var uri)
      && uri.Host == NAME_HOST
      && uri.Segments.LastOrDefault() is string name)
      ? name.Replace('-', ' ') : null;
  }
}