using MTGApplication.Models;
using System.Threading.Tasks;

namespace MTGApplication.ViewModels
{
  public class APISearchCardCollectionViewModel : MTGCardCollectionViewModel
  {
    public APISearchCardCollectionViewModel(MTGCardCollectionModel model) : base(model) { }

    /// <summary>
    /// Fetches cards from API and changes cards to the fetched cards
    /// </summary>
    /// <param name="query">API query parameters</param>
    /// <returns></returns>
    public virtual async Task Search(string query)
    {
      IsBusy = true;
      Clear();
      if (!string.IsNullOrEmpty(query))
      {
        var cards = await App.CardAPI.FetchCards(query);
        foreach (var item in cards)
        {
          Model.Add(item, false);
        }
      }
      IsBusy = false;
    }
  }
}
