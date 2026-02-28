using MTGApplication.General.Extensions;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.ScryfallAPI;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using MTGApplication.General.ViewModels;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.UseCases;

public class ImportCards(IMTGCardImporter importer, IEdhrecImporter edhrecImporter, IScryfallImporter scryfallImporter) : UseCaseFunc<string, Task<CardImportResult>>
{
  public override async Task<CardImportResult> Execute(string data)
  {
    return TryInternalImport(data)
        ?? await TryEdhrecImageImport(data)
        ?? await TryScryfallImageImport(data)
        ?? await CardImporterImport(data);
  }

  private CardImportResult? TryInternalImport(string json)
  {
    JsonExtensions.TryDeserializeJson<CardImportResult.Card>(json, out var card);

    if (card == null)
      return null;

    return new([card], 0, 1, CardImportResult.ImportSource.Internal);
  }

  private async Task<CardImportResult?> TryEdhrecImageImport(string uri)
  {
    if (edhrecImporter.TryParseCardNameFromEdhrecUri(uri, out var name))
      return await CardImporterImport(name);

    return null;
  }

  private async Task<CardImportResult?> TryScryfallImageImport(string uri)
  {
    var result = CardImportResult.Empty();

    if (ScryfallAPI.TryParseCardIdFromUri(uri, out var id))
      result = await scryfallImporter.ImportWithId(id);
    else if (ScryfallAPI.TryParseCardNameFromUri(uri, out var name))
      result = await scryfallImporter.ImportWithName(name, fuzzy: true);

    return result.TotalCount > 0 ? result : null;
  }

  private async Task<CardImportResult> CardImporterImport(string importText)
    => await new FetchCardsWithImportString(importer).Execute(importText);
}