using CommunityToolkit.Common.Collections;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public abstract class IncrementalCardSource<TCard>(MTGCardImporter importer) : object(), IIncrementalSource<TCard> where TCard : IMTGCard
{
  public List<TCard> Cards { get; set; } = [];
  public string NextPage { get; set; }
  public int PageSize => importer.PageSize;

  public async Task<IEnumerable<TCard>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
  {
    if (!string.IsNullOrEmpty(NextPage))
    {
      // Load next page
      var result = await importer.ImportFromUri(NextPage);
      NextPage = result.NextPageUri;
      Cards.AddRange(result.Found.Select(ConvertToCardType));
    }

    return (from card in Cards select card).Skip(pageIndex * pageSize).Take(pageSize);
  }

  protected abstract TCard ConvertToCardType(CardImportResult<MTGCardInfo>.Card card);
}