using ABI.System;
using Microsoft.Extensions.Caching.Memory;
using MTGApplication.General.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.General.Services.Importers.CardImporter.UseCases;

public class FetchCardsWithUri(IMTGCardImporter importer) : UseCaseFunc<string, Task<CardImportResult>>
{
  public bool FetchAll { get; init; } = false;
  public bool PaperOnly { get; init; } = true;
  public CancellationToken? CancellationToken { get; init; } = null;
  public IMemoryCache? Cache { get; init; } = null;

  /// <exception cref="Exception"></exception>
  public override async Task<CardImportResult> Execute(string uri)
  {
    var results = new List<CardImportResult>();

    if (Cache?.Get(uri) is CardImportResult cachedUri)
      results.Add(cachedUri);
    else
    {
      var result = await importer.ImportWithUri(uri, paperOnly: PaperOnly);

      if (result.Found.Length != 0)
        Cache?.Set(uri, result);

      results.Add(result);
    }

    while (FetchAll && results.LastOrDefault()?.NextPageUri is string page && !string.IsNullOrEmpty(page))
    {
      CancellationToken?.ThrowIfCancellationRequested();

      if (Cache?.Get(uri) is CardImportResult cachedPage)
        results.Add(cachedPage);
      else
      {
        var result = await importer.ImportWithUri(page, paperOnly: PaperOnly);

        if (result.Found.Length != 0)
          Cache?.Set(page, result);

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
