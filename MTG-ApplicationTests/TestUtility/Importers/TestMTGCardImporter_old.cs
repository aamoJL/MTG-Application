using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplicationTests.TestUtility.Mocker;
using static MTGApplication.General.Services.Importers.CardImporter.CardImportResult;

namespace MTGApplicationTests.TestUtility.Importers;

// TODO: remove
[Obsolete]
public class TestMTGCardImporter_old(Card[] expectedCards = null, int notFoundCount = 0) : IMTGCardImporter
{
  public Card[] ExpectedCards { get; set; } = expectedCards;
  public int NotFoundCount { get; set; } = notFoundCount;
  public Exception Exception { get; set; } = null;
  public TimeSpan Delay { get; set; } = TimeSpan.Zero;

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

public class TestMTGCardImporter : IMTGCardImporter
{
  public static CardImportResult Success(Card[] cards) => new(cards, 0, cards.Length, ImportSource.External);
  public static CardImportResult Failure() => new([], 5, 0, ImportSource.External);
  public static CardImportResult Partial(Card[] cards) => new(cards, 5, cards.Length + 5, ImportSource.External);

  public string Name => throw new NotImplementedException();

  public CardImportResult Result { get; init; } = null;
  /// <summary>
  /// <para>If set, import tasks will halt for 5 seconds, or when this source's token has been cancelled.</para>
  /// <para>Use this to unit test cancellable commands.</para>
  /// <para>Cancelling this token will NOT throw <see cref="OperationCanceledException"/></para>
  /// </summary>
  public CancellationTokenSource CancellationTokenSource { get; init; } = null;

  public async Task<CardImportResult> ImportCardsWithSearchQuery(string searchParams, bool pagination = true)
  {
    if (CancellationTokenSource != null)
      await WaitForCancellation(CancellationTokenSource.Token);

    if (Result == null) throw new NullReferenceException();

    return Result;
  }

  public async Task<CardImportResult> ImportWithDTOs(IEnumerable<MTGCardDTO> dtos)
  {
    if (CancellationTokenSource != null)
      await WaitForCancellation(CancellationTokenSource.Token);

    if (Result == null) throw new NullReferenceException();

    return Result;
  }

  public async Task<CardImportResult> ImportWithString(string importText)
  {
    if (CancellationTokenSource != null)
      await WaitForCancellation(CancellationTokenSource.Token);

    if (Result == null) throw new NullReferenceException();

    return Result;
  }

  public async Task<CardImportResult> ImportWithUri(string pageUri, bool paperOnly = false, bool fetchAll = false)
  {
    if (CancellationTokenSource != null)
      await WaitForCancellation(CancellationTokenSource.Token);

    if (Result == null) throw new NullReferenceException();

    return Result;
  }

  private static async Task WaitForCancellation(CancellationToken token)
  {
    try
    {
      await Task.Delay(5_000, token);
    }
    catch (OperationCanceledException) { }
  }
}