using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI;
using MTGApplication.API.CardAPI;
using MTGApplication.General.Models.Card;
using System.Collections.Generic;

namespace MTGApplication.General.ViewModels;

public partial class IncrementalLoadingCardCollection : ObservableObject
{
  public IncrementalLoadingCardCollection(ICardAPI<MTGCard> cardAPI)
  {
    CardAPI = cardAPI;
    Collection = new IncrementalLoadingCollection<IncrementalCardSource, MTGCard>(
      source: new IncrementalCardSource(CardAPI));
  }

  [ObservableProperty] private IncrementalLoadingCollection<IncrementalCardSource, MTGCard> collection;
  [ObservableProperty] private int totalCardCount;

  private ICardAPI<MTGCard> CardAPI { get; }

  public void SetCollection(List<MTGCard> cards, string nextPageUri, int totalCount)
  {
    TotalCardCount = totalCount;
    Collection = new(
      itemsPerPage: CardAPI.PageSize,
      source: new(CardAPI)
      {
        Cards = cards,
        NextPage = nextPageUri
      });
  }
}