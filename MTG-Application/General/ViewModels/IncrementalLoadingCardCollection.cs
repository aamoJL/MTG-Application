using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using MTGApplication.General.Models;
using System.Collections.Generic;

namespace MTGApplication.General.ViewModels;

public partial class IncrementalLoadingCardCollection<TCard>(IncrementalCardSource<TCard> source) : ObservableObject where TCard : MTGCard
{
  [ObservableProperty] public partial IncrementalLoadingCollection<IncrementalCardSource<TCard>, TCard> Collection { get; set; } = new(source: source, itemsPerPage: source.PageSize);
  [ObservableProperty] public partial int TotalCardCount { get; set; }

  private IncrementalCardSource<TCard> Source { get; } = source;

  public void SetCollection(List<TCard> cards, string nextPageUri, int totalCount)
  {
    TotalCardCount = totalCount;
    Source.NextPage = nextPageUri;
    Source.Cards = cards;

    Collection.RefreshAsync();
  }
}