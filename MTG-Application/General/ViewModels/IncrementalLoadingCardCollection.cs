using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using MTGApplication.General.Models.Card;
using System.Collections.Generic;

namespace MTGApplication.General.ViewModels;

public partial class IncrementalLoadingCardCollection<TCard>(IncrementalCardSource<TCard> source) : ObservableObject where TCard : MTGCard
{
  [ObservableProperty] private IncrementalLoadingCollection<IncrementalCardSource<TCard>, TCard> collection = new(source: source, itemsPerPage: source.CardAPI.PageSize);
  [ObservableProperty] private int totalCardCount;

  private IncrementalCardSource<TCard> Source { get; } = source;

  public void SetCollection(List<MTGCard> cards, string nextPageUri, int totalCount)
  {
    TotalCardCount = totalCount;
    Source.NextPage = nextPageUri;
    Source.Cards = cards;

    Collection.RefreshAsync();
  }
}