using MTGApplication.General.Services.Databases.Repositories.CardRepository.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using static MTGApplication.General.Services.Importers.CardImporter.CardImportResult;

namespace MTGApplicationTests.TestUtility.Importers;

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