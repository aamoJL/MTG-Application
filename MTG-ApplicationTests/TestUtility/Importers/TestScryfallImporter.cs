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
}
