using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Features.DeckEditor.Editor.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.Models;

/// <summary>
/// Class for MTG card decks
/// </summary>
public partial class DeckEditorMTGDeck : ObservableObject
{
  [ObservableProperty] private string name = "";
  [ObservableProperty] private DeckEditorMTGCard commander;
  [ObservableProperty] private DeckEditorMTGCard commanderPartner;

  public ObservableCollection<DeckEditorMTGCard> DeckCards { get; set; } = [];
  public ObservableCollection<DeckEditorMTGCard> Wishlist { get; set; } = [];
  public ObservableCollection<DeckEditorMTGCard> Maybelist { get; set; } = [];
  public ObservableCollection<DeckEditorMTGCard> Removelist { get; set; } = [];

  public int DeckSize => DeckCards.Sum(x => x.Count) + (Commander != null ? 1 : 0) + (CommanderPartner != null ? 1 : 0);
  public double DeckPrice => DeckCards.Sum(x => x.Info.Price * x.Count) + (Commander?.Info.Price ?? 0) + (CommanderPartner?.Info.Price ?? 0);
}