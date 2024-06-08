namespace MTGApplication.General.Models.Card;

public record CardImportResult(
  CardImportResult.Card[] Found,
  int NotFoundCount,
  int TotalCount,
  CardImportResult.ImportSource Source,
  string NextPageUri = "")
{
  public record Card(MTGCardInfo Info, int Count = 1);
  public enum ImportSource { Internal, External }

  /// <summary>
  /// Returns empty result object
  /// </summary>
  public static CardImportResult Empty(ImportSource source = ImportSource.External) => new([], 0, 0, source);
}