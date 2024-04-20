using CommunityToolkit.Common.Collections;
using MTGApplication.API.CardAPI;
using MTGApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels;

/// <summary>
/// Generic Source class for IncrementalLoadingCollection using <see cref="MTGCardViewModel"/>
/// </summary>
public abstract class MTGCardSource<T> : IIncrementalSource<T> where T : MTGCardViewModel
{
  public List<MTGCard> Cards { get; set; } = new();
  public ICardAPI<MTGCard> CardAPI { get; init; }
  public string NextPage { get; set; }

  #region IIncrementalSource implementation
  public async Task<IEnumerable<T>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
  {
    if (!string.IsNullOrEmpty(NextPage))
    {
      // Load next page
      var result = await CardAPI.FetchFromUri(NextPage);
      NextPage = result.NextPageUri;
      foreach (var card in result.Found)
      {
        Cards.Add(card);
      }
    }
    return await Task.Run(() => (from p in Cards select p).Skip(pageIndex * pageSize).Take(pageSize).Select(x => GetCardViewModel(x)));
  }
  #endregion

  /// <summary>
  /// Returns the given <paramref name="card"/> as a <typeparamref name="T"/>
  /// </summary>
  protected abstract T GetCardViewModel(MTGCard card);
}

/// <summary>
/// Source class for IncrementalLoadingCollection using <see cref="MTGCardViewModel"/>
/// </summary>
public class MTGCardViewModelSource : MTGCardSource<MTGCardViewModel>
{
  public MTGCardViewModelSource() : base() { }

  protected override MTGCardViewModel GetCardViewModel(MTGCard card) => new(card);
}

/// <summary>
/// Source class for IncrementalLoadingCollection using <see cref="MTGCardCollectionCardViewModel"/>
/// </summary>
public class MTGCardCollectionCardViewModelSource : MTGCardSource<MTGCardCollectionCardViewModel>
{
  public MTGCardCollectionCardViewModelSource() : base() { }

  protected override MTGCardCollectionCardViewModel GetCardViewModel(MTGCard card) => new(card);
}
