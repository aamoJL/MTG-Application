using CommunityToolkit.WinUI.Collections;
using MTGApplication.General.Services.Importers.CardImporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public class IncrementalCardSource<TCard>(IMTGCardImporter importer) : object(), IIncrementalSource<TCard>
{
  public List<TCard> Cards { get; set; } = [];
  public IMTGCardImporter Importer { get; } = importer;
  public string NextPage { get; set; } = string.Empty;
  public required Func<CardImportResult.Card, TCard> Converter { private get; init; }

  public Action<Exception> OnError { get; set; } = (_) => { };

  public async Task<IEnumerable<TCard>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
  {
    var startIndex = pageIndex * pageSize;

    if (!string.IsNullOrEmpty(NextPage) && startIndex >= Cards.Count)
    {
      // Load next page
      try
      {
        var result = await Importer.ImportWithUri(NextPage);
        NextPage = result.NextPageUri;
        Cards.AddRange(result.Found.Select(Converter));
      }
      catch (Exception e)
      {
        OnError?.Invoke(e);
      }
    }

    return (from card in Cards select card).Skip(startIndex).Take(pageSize);
  }
}