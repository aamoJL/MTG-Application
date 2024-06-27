using MTGApplication.General.Models;

namespace MTGApplication.General.Services.Importers.CardImporter;

public record CardImportResult(
  CardImportResult.Card[] Found,
  int NotFoundCount,
  int TotalCount,
  CardImportResult.ImportSource Source,
  string NextPageUri = "")
{
  public enum ImportSource { Internal, External }

  public record Card(MTGCardInfo Info, int Count = 1);

  /// <summary>
  /// Returns empty result object
  /// </summary>
  public static CardImportResult Empty(ImportSource source = ImportSource.External) => new([], 0, 0, source);
}