using CommunityToolkit.Common.Collections;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.Services
{
  public abstract class MTGCardSource<T> : IIncrementalSource<T> where T : MTGCardViewModel
  {
    public List<MTGCard> Cards { get; set; } = new();
    public ICardAPI<MTGCard> CardAPI { get; init; }
    public string NextPage { get; set; }

    public async Task<IEnumerable<T>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
    {
      if (!string.IsNullOrEmpty(NextPage))
      {
        // Load next page
        (var newCards, NextPage, _) = await CardAPI.FetchCardsFromPage(NextPage);
        foreach (var card in newCards)
        {
          Cards.Add(card);
        }
      }
      return await Task.Run(() => (from p in Cards select p).Skip(pageIndex * pageSize).Take(pageSize).Select(x => GetCardViewModel(x)));
    }

    protected abstract T GetCardViewModel(MTGCard card);
  }
}
