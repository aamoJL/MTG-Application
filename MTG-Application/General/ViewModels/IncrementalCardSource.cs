using CommunityToolkit.Common.Collections;
using MTGApplication.API.CardAPI;
using MTGApplication.General.Models.Card;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public class IncrementalCardSource : IIncrementalSource<MTGCard>
{
  public IncrementalCardSource(ICardAPI<MTGCard> cardAPI) : base() => CardAPI = cardAPI;

  public List<MTGCard> Cards { get; set; } = new();
  public ICardAPI<MTGCard> CardAPI { get; init; }
  public string NextPage { get; set; }

  public async Task<IEnumerable<MTGCard>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
  {
    if (!string.IsNullOrEmpty(NextPage))
    {
      // Load next page
      var result = await CardAPI.FetchFromUri(NextPage);
      NextPage = result.NextPageUri;

      foreach (var card in result.Found)
        Cards.Add(card);
    }

    return (from card in Cards select card).Skip(pageIndex * pageSize).Take(pageSize).Select(x => x);
  }
}
