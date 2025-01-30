using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.General.Services.Importers.CardImporter.CardImportResult;

namespace MTGApplicationTests.TestUtility.Importers;
public class TestMTGCardImporter(Card[] expectedCards = null, int notFoundCount = 0) : IMTGCardImporter
{
  public Card[] ExpectedCards { get; set; } = expectedCards;
  public int NotFoundCount { get; set; } = notFoundCount;
  public Exception Exception { get; set; } = null;
  public TimeSpan Delay { get; set; } = TimeSpan.Zero;

  public int PageSize => 40;
  public string Name => "Test Card API";

  public async Task<CardImportResult> ImportCardsWithSearchQuery(string searchParams, bool pagination = true)
  {
    if (Exception != null)
      throw Exception;

    await Task.Delay(Delay);

    return string.IsNullOrEmpty(searchParams)
      ? Empty(ImportSource.External)
      : await Task.Run(() => ExpectedCards != null
      ? new CardImportResult(ExpectedCards, NotFoundCount, ExpectedCards!.Length, ImportSource.External)
      : Empty(ImportSource.External));
  }

  public async Task<CardImportResult> ImportWithDTOs(IEnumerable<MTGCardDTO> dtoArray)
  {
    if (Exception != null)
      throw Exception;

    var cards = dtoArray.Select(x => new Card(MTGCardInfoMocker.FromDTO(x), x.Count)).ToArray();

    await Task.Delay(Delay);

    if (ExpectedCards == null) { return await Task.Run(() => new CardImportResult(cards, 0, cards.Length, ImportSource.External)); }
    else
    {
      var found = ExpectedCards!.Where(ex => cards.FirstOrDefault(x => x.Info.ScryfallId == ex.Info.ScryfallId) != null)?.ToList() ?? [];
      var notFoundCount = ExpectedCards!.Length - found.Count;

      return await Task.Run(() => new CardImportResult([.. found], notFoundCount, found.Count, ImportSource.External));
    }
  }

  public async Task<CardImportResult> ImportWithString(string importText)
  {
    if (Exception != null)
      throw Exception;

    await Task.Delay(Delay);

    return await Task.Run(() => ExpectedCards != null ? new CardImportResult(ExpectedCards, NotFoundCount, ExpectedCards!.Length, ImportSource.External) : Empty());
  }

  public async Task<CardImportResult> ImportWithUri(string pageUri, bool paperOnly = false, bool fetchAll = false)
  {
    if (Exception != null)
      throw Exception;

    var cards = string.IsNullOrEmpty(pageUri) ? [] : ExpectedCards ?? [];

    await Task.Delay(Delay);

    return await Task.Run(() => new CardImportResult(cards, NotFoundCount, cards.Length, ImportSource.External));
  }
}