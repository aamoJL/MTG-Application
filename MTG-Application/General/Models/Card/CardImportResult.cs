using System;

namespace MTGApplication.General.Models.Card;

public record CardImportResult(
  MTGCard[] Found,
  int NotFoundCount,
  int TotalCount,
  string NextPageUri = "")
{
  /// <summary>
  /// Returns empty result object
  /// </summary>
  public static CardImportResult Empty() => new(Array.Empty<MTGCard>(), 0, 0);
}