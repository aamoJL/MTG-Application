namespace MTGApplication.General.Services.Importers.CardImporter;

public abstract record CardImportResult
{
  public enum ImportSource { Internal, External }
}

public record CardImportResult<TInfo>(
  CardImportResult<TInfo>.Card[] Found,
  int NotFoundCount,
  int TotalCount,
  CardImportResult.ImportSource Source,
  string NextPageUri = "") : CardImportResult
{
  public record Card(TInfo Info, int Count = 1);

  /// <summary>
  /// Returns empty result object
  /// </summary>
  public static CardImportResult<TInfo> Empty(ImportSource source = ImportSource.External) => new([], 0, 0, source);
}