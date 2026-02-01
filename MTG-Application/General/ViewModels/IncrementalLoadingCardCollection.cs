using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Collections;
using MTGApplication.General.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public partial class IncrementalLoadingCardCollection<TCard>(IncrementalCardSource<TCard> source) : ObservableObject where TCard : MTGCard
{
  public IncrementalLoadingCollection<IncrementalCardSource<TCard>, TCard> Collection { get; } = new(source: source, itemsPerPage: source.PageSize);

  [ObservableProperty] public partial int TotalCardCount { get; set; }

  private IncrementalCardSource<TCard> Source { get; } = source;

  public async Task SetCollection(List<TCard> cards, string nextPageUri, int totalCount)
  {
    TotalCardCount = totalCount;
    Source.NextPage = nextPageUri;
    Source.Cards = cards;

    await Collection.RefreshAsync();
  }
}