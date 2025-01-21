using MTGApplication.General.Extensions;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Services.Importers.CardImporter.UseCases;
using System.Threading.Tasks;

namespace MTGApplication.Features.DeckEditor.Editor.Services;

public class DeckEditorCardImporter(IMTGCardImporter importer)
{
  /// <exception cref="System.InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="System.UriFormatException"></exception>
  public async Task<CardImportResult> Import(string data)
  {
    try
    {
      return TryInternalImport(data)
        ?? await TryEdhrecImageImport(data)
        ?? await TryScryfallImageImport(data)
        ?? await ApiImport(data);
    }
    catch { throw; }
  }

  private CardImportResult? TryInternalImport(string data)
  {
    if (JsonExtensions.TryDeserializeJson<CardImportResult.Card>(data, out var card) && card != null)
      return new([new CardImportResult.Card(card.Info, card.Count)], 0, 1, CardImportResult.ImportSource.Internal);
    else
      return null;
  }

  private async Task<CardImportResult?> TryEdhrecImageImport(string data)
  {
    try
    {
      _ = EdhrecImporter.TryParseCardNameFromEdhrecUri(data, out var name);

      return name != null ? await new FetchCardsWithImportString(importer).Execute(name) : null;
    }
    catch { throw; }
  }

  private async Task<CardImportResult?> TryScryfallImageImport(string data)
  {
    try
    {
      return await ScryfallImporter.TryImportFromUri(data);
    }
    catch { throw; }
  }

  private async Task<CardImportResult> ApiImport(string data)
  {
    try
    {
      return await new FetchCardsWithImportString(importer).Execute(data);
    }
    catch { throw; }
  }
}