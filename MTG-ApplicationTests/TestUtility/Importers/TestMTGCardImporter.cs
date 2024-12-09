using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.General.Services.Importers.CardImporter.CardImportResult;

namespace MTGApplicationTests.TestUtility.Importers;
public class TestMTGCardImporter(Card[] expectedCards = null, int notFoundCount = 0) : MTGCardImporter
{
  public Card[] ExpectedCards { get; set; } = expectedCards;
  public int NotFoundCount { get; set; } = notFoundCount;

  public override int PageSize => 40;
  public override string Name => "Test Card API";

  public override async Task<CardImportResult> ImportCardsWithSearchQuery(string searchParams, bool pagination = true)
  {
    return string.IsNullOrEmpty(searchParams)
      ? Empty(ImportSource.External)
      : await Task.Run(() => ExpectedCards != null
      ? new CardImportResult(ExpectedCards, NotFoundCount, ExpectedCards!.Length, ImportSource.External)
      : Empty(ImportSource.External));
  }

  public override async Task<CardImportResult> ImportFromDTOs(IEnumerable<MTGCardDTO> dtoArray)
  {
    var cards = dtoArray.Select(x => new Card(MTGCardInfoMocker.FromDTO(x), x.Count)).ToArray();

    if (ExpectedCards == null) { return await Task.Run(() => new CardImportResult(cards, 0, cards.Length, ImportSource.External)); }
    else
    {
      var found = ExpectedCards!.Where(ex => cards.FirstOrDefault(x => x.Info.ScryfallId == ex.Info.ScryfallId) != null)?.ToList() ?? [];
      var notFoundCount = ExpectedCards!.Length - found.Count;

      return await Task.Run(() => new CardImportResult([.. found], notFoundCount, found.Count, ImportSource.External));
    }
  }

  public override async Task<CardImportResult> ImportFromString(string importText)
    => await Task.Run(() => ExpectedCards != null ? new CardImportResult(ExpectedCards, NotFoundCount, ExpectedCards!.Length, ImportSource.External) : Empty());

  public override async Task<CardImportResult> ImportFromUri(string pageUri, bool paperOnly = false, bool fetchAll = false)
  {
    var cards = string.IsNullOrEmpty(pageUri) ? [] : ExpectedCards ?? [];
    return await Task.Run(() => new CardImportResult(cards, NotFoundCount, cards.Length, ImportSource.External));
  }
}