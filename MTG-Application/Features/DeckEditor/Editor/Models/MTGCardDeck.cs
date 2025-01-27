using MTGApplication.Features.DeckEditor.Editor.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MTGApplication.Features.DeckEditor.Models;

/// <summary>
/// Class for MTG card decks
/// </summary>
public partial class DeckEditorMTGDeck : INotifyPropertyChanged, INotifyPropertyChanging
{
  public string Name
  {
    get;
    set
    {
      if (field != value)
      {
        PropertyChanging?.Invoke(this, new(nameof(Name)));
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(Name)));
      }
    }
  } = "";
  public DeckEditorMTGCard? Commander
  {
    get;
    set
    {
      if (field != value)
      {
        PropertyChanging?.Invoke(this, new(nameof(Commander)));
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(Commander)));
      }
    }
  }
  public DeckEditorMTGCard? CommanderPartner
  {
    get;
    set
    {
      if (field != value)
      {
        PropertyChanging?.Invoke(this, new(nameof(CommanderPartner)));
        field = value;
        PropertyChanged?.Invoke(this, new(nameof(CommanderPartner)));
      }
    }
  }

  public ObservableCollection<DeckEditorMTGCard> DeckCards { get; set; } = [];
  public ObservableCollection<DeckEditorMTGCard> Wishlist { get; set; } = [];
  public ObservableCollection<DeckEditorMTGCard> Maybelist { get; set; } = [];
  public ObservableCollection<DeckEditorMTGCard> Removelist { get; set; } = [];

  public event PropertyChangedEventHandler? PropertyChanged;
  public event PropertyChangingEventHandler? PropertyChanging;
}