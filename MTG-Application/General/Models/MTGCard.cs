using CommunityToolkit.Mvvm.ComponentModel;

namespace MTGApplication.General.Models;

public partial class MTGCard(MTGCardInfo info) : ObservableObject
{
  [ObservableProperty] private MTGCardInfo info = info;
}