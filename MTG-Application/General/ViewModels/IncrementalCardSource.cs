using CommunityToolkit.Common.Collections;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public class IncrementalCardSource(ICardAPI<MTGCard> cardAPI) : object(), IIncrementalSource<MTGCard>
{
  public List<MTGCard> Cards { get; set; } = [];
  public ICardAPI<MTGCard> CardAPI { get; init; } = cardAPI;
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
