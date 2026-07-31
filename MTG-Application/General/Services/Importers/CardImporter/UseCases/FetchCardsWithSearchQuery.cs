using MTGApplication.General.Services.Cache;
using MTGApplication.General.Services.Cache.Caches;
using MTGApplication.General.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter.UseCases;

public class FetchCardsWithSearchQuery(IMTGCardImporter importer) : UseCaseFunc<string, Task<CardImportResult>>
{
  public bool FetchAll { get; init; } = false;
  public CancellationToken? CancellationToken { get; init; } = null;
  public IMemoryCache<Caching.CacheKey>? Cache { get; init; } = null;

  /// <exception cref="InvalidOperationException"></exception>
  /// <exception cref="System.Net.Http.HttpRequestException"></exception>
  /// <exception cref="UriFormatException"></exception>
  /// <exception cref="System.Text.Json.JsonException"></exception>
  public async override Task<CardImportResult> Execute(string query)
  {
    var results = new List<CardImportResult>();

    if (Cache?.TryGetImportResult(query, out var cachedQuery) is true)
      results.Add(cachedQuery);
    else
    {
      var result = await importer.ImportCardsWithSearchQuery(query);

      if (result.Found.Length != 0)
        Cache?.CacheImportResult(query, result);

      results.Add(result);
    }

    while (FetchAll && results.LastOrDefault()?.NextPageUri is string page && !string.IsNullOrEmpty(page))
    {
      CancellationToken?.ThrowIfCancellationRequested();

      if (Cache?.TryGetImportResult(page, out var cachedPage) is true)
        results.Add(cachedPage);
      else
      {
        var result = await importer.ImportWithUri(page);

        if (result.Found.Length != 0)
          Cache?.CacheImportResult(page, result);

        results.Add(result);
      }
    }

    CancellationToken?.ThrowIfCancellationRequested();

    if (results.Count == 0)
      return CardImportResult.Empty();

    return new(
      Found: [.. results.SelectMany(x => x.Found)],
      NotFoundCount: results.Sum(x => x.NotFoundCount),
      TotalCount: results.First().TotalCount,
      NextPageUri: results.Last().NextPageUri,
      Source: CardImportResult.ImportSource.External);
  }
}