using CommunityToolkit.Common.Collections;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services.API.CardAPI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public abstract class IncrementalCardSource<TCard>(ICardAPI<DeckEditorMTGCard> cardAPI) : object(), IIncrementalSource<TCard> where TCard : DeckEditorMTGCard
{
  public List<DeckEditorMTGCard> Cards { get; set; } = [];
  public string NextPage { get; set; }

  public ICardAPI<DeckEditorMTGCard> CardAPI { get; } = cardAPI;

  public async Task<IEnumerable<TCard>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
  {
    if (!string.IsNullOrEmpty(NextPage))
    {
      // Load next page
      var result = await CardAPI.FetchFromUri(NextPage);
      NextPage = result.NextPageUri;
      Cards.AddRange(result.Found.Select(x => new DeckEditorMTGCard(x.Info, x.Count)));
    }

    return (from card in Cards select card).Skip(pageIndex * pageSize).Take(pageSize).Select(ConvertToCardType);
  }

  protected abstract TCard ConvertToCardType(DeckEditorMTGCard card);
}