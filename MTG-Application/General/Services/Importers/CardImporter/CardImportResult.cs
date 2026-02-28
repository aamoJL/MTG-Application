using MTGApplication.General.Models;
using System.Text.Json.Serialization;

namespace MTGApplication.General.Services.Importers.CardImporter;

public record CardImportResult(
  CardImportResult.Card[] Found,
  int NotFoundCount,
  int TotalCount,
  CardImportResult.ImportSource Source,
  string NextPageUri = "")
{
  public enum ImportSource { Internal, External }

  public record Card
  {
    [JsonConstructor]
    public Card(MTGCardInfo Info) => this.Info = Info;

    public MTGCardInfo Info { get; init; }
    public int Count { get; init; } = 1;
  };

  /// <summary>
  /// Returns empty result object
  /// </summary>
  public static CardImportResult Empty(ImportSource source = ImportSource.External) => new([], 0, 0, source);
}