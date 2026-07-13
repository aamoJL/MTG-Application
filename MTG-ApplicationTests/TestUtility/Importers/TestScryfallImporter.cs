using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.ScryfallAPI;

namespace MTGApplicationTests.TestUtility.Importers;

public class TestScryfallImporter : IScryfallImporter
{
  public CardImportResult? Result { get; init; } = null;

  public async Task<CardImportResult> ImportWithId(Guid id)
  {
    if (Result == null) throw new NotImplementedException($"ImportWithId {nameof(Result)}");

    return Result;
  }

  public async Task<CardImportResult> ImportWithName(string name, bool fuzzy)
  {
    if (Result == null) throw new NotImplementedException($"ImportWithName {nameof(Result)}");

    return Result;
  }

  public bool TryParseCardIdFromUri(string data, out Guid id)
  {
    Guid? result = (Uri.TryCreate(data, UriKind.Absolute, out var uri)
      && uri.Host == "cards.scryfall.io"
      && uri.Segments.LastOrDefault() is string imageFileName
      && Path.GetFileNameWithoutExtension(imageFileName) is string idString
      && Guid.TryParse(idString, out var parsedId))
      ? parsedId : null;

    id = result != null ? (Guid)result : default;

    return result != null;
  }

  public bool TryParseCardNameFromUri(string data, out string name)
  {
    var result = (Uri.TryCreate(data, UriKind.Absolute, out var uri)
      && uri.Host == "scryfall.com"
      && uri.Segments.LastOrDefault() is string parsedName)
      ? parsedName.Replace('-', ' ') : null;

    name = result ?? string.Empty;

    return result != null;
  }
}
