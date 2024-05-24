using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Models.Card;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.General.Models.CardDeck;

/// <summary>
/// Class for MTG card decks
/// </summary>
public partial class MTGCardDeck : ObservableObject
{
  [ObservableProperty] private string name = "";
  [ObservableProperty] private MTGCard commander;
  [ObservableProperty] private MTGCard commanderPartner;

  public ObservableCollection<MTGCard> DeckCards { get; set; } = [];
  public ObservableCollection<MTGCard> Wishlist { get; set; } = [];
  public ObservableCollection<MTGCard> Maybelist { get; set; } = [];
  public ObservableCollection<MTGCard> Removelist { get; set; } = [];

  public int DeckSize => DeckCards.Sum(x => x.Count) + (Commander != null ? 1 : 0) + (CommanderPartner != null ? 1 : 0);
  public double DeckPrice => DeckCards.Sum(x => x.Info.Price * x.Count) + (Commander?.Info.Price ?? 0) + (CommanderPartner?.Info.Price ?? 0);
}