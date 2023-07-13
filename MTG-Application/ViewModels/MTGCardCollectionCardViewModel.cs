using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Models;

namespace MTGApplication.ViewModels;

public partial class MTGCardCollectionCardViewModel : MTGCardViewModel
{
  public MTGCardCollectionCardViewModel(MTGCard model) : base(model) { }

  [ObservableProperty]
  private bool isOwned;
}
