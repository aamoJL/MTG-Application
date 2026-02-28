using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.Collections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTGApplication.General.ViewModels;

public partial class IncrementalLoadingCardCollection<TCard>(IncrementalCardSource<TCard> source) : ObservableObject
{
  public IncrementalLoadingCollection<IncrementalCardSource<TCard>, TCard> Collection { get; private set; } = new(source: source);

  [ObservableProperty] public partial int TotalCardCount { get; set; }

  private IncrementalCardSource<TCard> Source { get; } = source;

  [Obsolete("Set new IncrementalLoadingCardCollection")]
  public async Task SetCollection(List<TCard> cards, string nextPageUri, int totalCount)
  {
    TotalCardCount = totalCount;
    Source.NextPage = nextPageUri;
    Source.Cards = cards;

    await Collection.RefreshAsync();
  }
}