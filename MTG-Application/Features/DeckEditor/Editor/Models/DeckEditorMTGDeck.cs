using MTGApplication.Features.DeckEditor.Editor.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MTGApplication.Features.DeckEditor.Models;

/// <summary>
/// Class for MTG card decks
/// </summary>
public partial class DeckEditorMTGDeck : INotifyPropertyChanged, INotifyPropertyChanging
{
  public string Name
  {
    get;
    set => SetProperty(ref field, value);
  } = "";
  public DeckEditorMTGCard? Commander
  {
    get;
    set => SetProperty(ref field, value);
  }
  public DeckEditorMTGCard? CommanderPartner
  {
    get;
    set => SetProperty(ref field, value);
  }

  public ObservableCollection<DeckEditorMTGCard> DeckCards { get; set; } = [];
  public ObservableCollection<DeckEditorMTGCard> Wishlist { get; set; } = [];
  public ObservableCollection<DeckEditorMTGCard> Maybelist { get; set; } = [];
  public ObservableCollection<DeckEditorMTGCard> Removelist { get; set; } = [];

  public event PropertyChangedEventHandler? PropertyChanged;
  public event PropertyChangingEventHandler? PropertyChanging;

  private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
  {
    if (!EqualityComparer<T>.Default.Equals(field, value))
    {
      PropertyChanging?.Invoke(this, new(propertyName));
      field = value;
      PropertyChanged?.Invoke(this, new(propertyName));
    }
  }
}