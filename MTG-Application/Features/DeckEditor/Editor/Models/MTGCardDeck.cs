using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Features.DeckEditor.Editor.Models;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckEditor.Models;

/// <summary>
/// Class for MTG card decks
/// </summary>
public partial class DeckEditorMTGDeck : ObservableObject
{
  [ObservableProperty] public partial string Name { get; set; } = "";
  [ObservableProperty] public partial DeckEditorMTGCard? Commander { get; set; }
  [ObservableProperty] public partial DeckEditorMTGCard? CommanderPartner { get; set; }

  public ObservableCollection<DeckEditorMTGCard> DeckCards { get; set; } = [];
  public ObservableCollection<DeckEditorMTGCard> Wishlist { get; set; } = [];
  public ObservableCollection<DeckEditorMTGCard> Maybelist { get; set; } = [];
  public ObservableCollection<DeckEditorMTGCard> Removelist { get; set; } = [];
}