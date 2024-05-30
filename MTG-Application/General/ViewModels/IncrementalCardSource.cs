using CommunityToolkit.Common.Collections;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public abstract class IncrementalCardSource<TCard>(ICardAPI<MTGCard> cardAPI) : object(), IIncrementalSource<TCard> where TCard : MTGCard
{
  public List<MTGCard> Cards { get; set; } = [];
  public string NextPage { get; set; }
  
  public ICardAPI<MTGCard> CardAPI { get; } = cardAPI;

  public async Task<IEnumerable<TCard>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
  {
    if (!string.IsNullOrEmpty(NextPage))
    {
      // Load next page
      var result = await CardAPI.FetchFromUri(NextPage);
      NextPage = result.NextPageUri;

      foreach (var card in result.Found)
        Cards.Add(card);
    }

    return (from card in Cards select card).Skip(pageIndex * pageSize).Take(pageSize).Select(ConvertToCardType);
  }

  protected abstract TCard ConvertToCardType(MTGCard card);
}