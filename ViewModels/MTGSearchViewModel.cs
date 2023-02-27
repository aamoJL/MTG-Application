using CommunityToolkit.Common.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels
{
  public partial class MTGSearchViewModel : ObservableObject
  {
    public class MTGCardSource : IIncrementalSource<MTGCardViewModel>
    {
      private readonly MTGCard[] cardArray;

      public MTGCardSource() { cardArray = Array.Empty<MTGCard>(); }
      public MTGCardSource(MTGCard[] cardArray)
      {
        this.cardArray = cardArray;
      }

      public async Task<IEnumerable<MTGCardViewModel>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
      {
        var result = await Task.Run(() => (from p in cardArray select p).Skip(pageIndex * pageSize).Take(pageSize).Select(x => new MTGCardViewModel(x)));

        return result;
      }
    }

    public MTGSearchViewModel(ICardAPI<MTGCard> cardAPI)
    {
      this.cardAPI = cardAPI;
    }

    private readonly ICardAPI<MTGCard> cardAPI;

    [ObservableProperty]
    private bool isBusy;
    [ObservableProperty]
    private IncrementalLoadingCollection<MTGCardSource, MTGCardViewModel> searchCards = new();
    [ObservableProperty]
    private int totalCardCount;

    public string SearchQuery { get; set; }


    /// <summary>
    /// Fetches cards from API using selected query, and replaces current cards with the fetched cards
    /// </summary>
    [RelayCommand]
    public async Task SearchSubmit()
    {
      IsBusy = true;
      var cards = await Task.Run(() => cardAPI.FetchCards(SearchQuery));
      SearchCards = new(new MTGCardSource(cards));
      TotalCardCount = cards.Length;
      IsBusy = false;
    }
  }
}
