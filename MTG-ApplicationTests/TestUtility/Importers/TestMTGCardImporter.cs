using MTGApplication.General.Models;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.General.Services.Importers.CardImporter.CardImportResult;

namespace MTGApplicationTests.TestUtility.API;
public class TestMTGCardImporter(CardImportResult<MTGCardInfo>.Card[]? expectedCards = null, int notFoundCount = 0) : MTGCardImporter
{
  public CardImportResult<MTGCardInfo>.Card[]? ExpectedCards { get; set; } = expectedCards;
  public int NotFoundCount { get; set; } = notFoundCount;

  public override int PageSize => 40;
  public override string Name => "Test Card API";

  public override async Task<CardImportResult<MTGCardInfo>> ImportCardsWithSearchQuery(string searchParams)
  {
    return string.IsNullOrEmpty(searchParams)
      ? CardImportResult<MTGCardInfo>.Empty(ImportSource.External)
      : await Task.Run(() => ExpectedCards != null
      ? new CardImportResult<MTGCardInfo>(ExpectedCards, NotFoundCount, ExpectedCards!.Length, ImportSource.External)
      : CardImportResult<MTGCardInfo>.Empty(ImportSource.External));
  }

  public override async Task<CardImportResult<MTGCardInfo>> ImportFromDTOs(IEnumerable<MTGCardDTO> dtoArray)
  {
    var cards = dtoArray.Select(x => new CardImportResult<MTGCardInfo>.Card(MTGCardInfoMocker.FromDTO(x), x.Count)).ToArray();

    if (ExpectedCards == null) { return await Task.Run(() => new CardImportResult<MTGCardInfo>(cards, 0, cards.Length, ImportSource.External)); }
    else
    {
      var found = ExpectedCards!.Where(ex => cards.FirstOrDefault(x => x.Info.ScryfallId == ex.Info.ScryfallId) != null)?.ToList() ?? [];
      var notFoundCount = ExpectedCards!.Length - found.Count;

      return await Task.Run(() => new CardImportResult<MTGCardInfo>([.. found], notFoundCount, found.Count, ImportSource.External));
    }
  }

  public override async Task<CardImportResult<MTGCardInfo>> ImportFromString(string importText)
    => await Task.Run(() => ExpectedCards != null ? new CardImportResult<MTGCardInfo>(ExpectedCards, NotFoundCount, ExpectedCards!.Length, ImportSource.External) : CardImportResult<MTGCardInfo>.Empty());

  public override async Task<CardImportResult<MTGCardInfo>> ImportFromUri(string pageUri, bool paperOnly = false, bool fetchAll = false)
  {
    var cards = string.IsNullOrEmpty(pageUri) ? [] : ExpectedCards ?? [];
    return await Task.Run(() => new CardImportResult<MTGCardInfo>(cards, NotFoundCount, cards.Length, ImportSource.External));
  }
}