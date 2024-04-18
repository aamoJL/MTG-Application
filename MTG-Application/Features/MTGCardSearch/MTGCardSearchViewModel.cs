using CommunityToolkit.Mvvm.Input;
using MTGApplication.Interfaces;
using MTGApplication.Models;
using MTGApplication.ViewModels;

namespace MTGApplication.Features.CardSearch;
public partial class MTGCardSearchViewModel : ViewModelBase
{
  public MTGCardSearchViewModel(ICardAPI<MTGCard> cardAPI)
  {
    
  }

  [RelayCommand]
  public void SubmitSearch(string query)
  {

  }
}
