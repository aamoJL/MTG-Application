using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Models;

namespace MTGApplication.ViewModels;

/// <summary>
/// View model for MTG card collections cards
/// </summary>
public partial class MTGCardCollectionCardViewModel : MTGCardViewModel
{
  public MTGCardCollectionCardViewModel(MTGCard model) : base(model) { }

  [ObservableProperty] private bool isOwned;
}
