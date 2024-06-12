using CommunityToolkit.Mvvm.ComponentModel;

namespace MTGApplication.General.Models;

public class MTGCard(MTGCardInfo info) : ObservableObject, IMTGCard
{
  public MTGCardInfo Info { get; set; } = info;
}
