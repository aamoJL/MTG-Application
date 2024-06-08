using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.General.Models.Card.CardImportResult;

namespace MTGApplicationTests.TestUtility.API;
public class TestCardAPI(DeckEditorMTGCard[]? expectedCards = null, int notFoundCount = 0) : ICardAPI<DeckEditorMTGCard>
{
  public DeckEditorMTGCard[]? ExpectedCards { get; set; } = expectedCards;
  public int NotFoundCount { get; set; } = notFoundCount;

  public int PageSize => 40;
  public string Name => "Test Card API";

  public async Task<CardImportResult> FetchCardsWithSearchQuery(string searchParams)
  {
    if (string.IsNullOrEmpty(searchParams)) { return Empty(); }
    return await Task.Run(() => ExpectedCards != null ? new CardImportResult(ExpectedCards, NotFoundCount, ExpectedCards!.Length, ImportSource.External) : Empty());
  }

  public async Task<CardImportResult> FetchFromDTOs(CardDTO[] dtoArray)
  {
    var cards = dtoArray.Select(x => MTGCardModelMocker.FromDTO((MTGCardDTO)x)).ToArray();

    if (ExpectedCards == null) { return await Task.Run(() => new CardImportResult(cards, 0, cards.Length, ImportSource.External)); }
    else
    {
      var found = ExpectedCards!.Where(ex => cards.FirstOrDefault(x => x.Info.ScryfallId == ex.Info.ScryfallId) != null)?.ToList() ?? new List<DeckEditorMTGCard>();
      var notFoundCount = ExpectedCards!.Length - found.Count;

      return await Task.Run(() => new CardImportResult([.. found], notFoundCount, found.Count, ImportSource.External));
    }
  }

  public async Task<CardImportResult> FetchFromString(string importText)
    => await Task.Run(() => ExpectedCards != null ? new CardImportResult(ExpectedCards, NotFoundCount, ExpectedCards!.Length, ImportSource.External) : Empty());

  public async Task<CardImportResult> FetchFromUri(string pageUri, bool paperOnly = false, bool fetchAll = false)
  {
    var cards = string.IsNullOrEmpty(pageUri) ? [] : ExpectedCards ?? [];
    return await Task.Run(() => new CardImportResult(cards, NotFoundCount, cards.Length, ImportSource.External));
  }
}