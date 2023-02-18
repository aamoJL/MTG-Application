using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels
{
    public partial class MTGSearchViewModel : ObservableObject
  {
    public MTGSearchViewModel(ICardAPI<MTGCard> cardAPI)
    {
      this.cardAPI = cardAPI;
    }

    private readonly ICardAPI<MTGCard> cardAPI;
    
    [ObservableProperty]
    private bool isBusy;

    public ObservableCollection<MTGCardViewModel> SearchCards { get; set; } = new();
    public string SearchQuery { get; set; }

    /// <summary>
    /// Fetches cards from API using selected query, and replaces current cards with the fetched cards
    /// </summary>
    [RelayCommand]
    public async Task SearchSubmit()
    {
      IsBusy = true;
      SearchCards.Clear();
      var cards = await Task.Run(() => cardAPI.FetchCards(SearchQuery));
      foreach (var item in cards)
      {
        SearchCards.Add(new(item));
      }
      IsBusy = false;
    }
  }
}
