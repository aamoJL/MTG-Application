using System;

namespace MTGApplication.General.Models.Card;

public record CardImportResult(
  MTGCard[] Found,
  int NotFoundCount,
  int TotalCount,
  CardImportResult.ImportSource Source,
  string NextPageUri = "")
{
  public enum ImportSource { Internal, External }

  /// <summary>
  /// Returns empty result object
  /// </summary>
  public static CardImportResult Empty(ImportSource source = ImportSource.External) => new(Array.Empty<MTGCard>(), 0, 0, source);
}