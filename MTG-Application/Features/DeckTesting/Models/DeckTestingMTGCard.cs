using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models;

namespace MTGApplication.Features.DeckTesting.Models;
public partial class DeckTestingMTGCard(MTGCardInfo info) : MTGCard(info)
{
  [ObservableProperty] public partial bool IsTapped { get; set; }

  public bool IsToken { get; init; } = false;
}