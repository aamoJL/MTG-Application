using MTGApplication.General.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MTGApplication.Features.DeckEditor.Models;

/// <summary>
/// Class for MTG card decks
/// </summary>
public partial class DeckEditorMTGDeck : INotifyPropertyChanged, INotifyPropertyChanging
{
  public DeckEditorMTGDeck() => PropertyChanged += Deck_PropertyChanged;

  public string Name
  {
    get;
    set => SetProperty(ref field, value);
  } = "";
  public DeckEditorMTGCard? Commander
  {
    get;
    set
    {
      field?.PropertyChanged -= DeckCard_PropertyChanged;
      SetProperty(ref field, value);
      field?.PropertyChanged += DeckCard_PropertyChanged;
    }
  }
  public DeckEditorMTGCard? CommanderPartner
  {
    get;
    set
    {
      field?.PropertyChanged -= DeckCard_PropertyChanged;
      SetProperty(ref field, value);
      field?.PropertyChanged += DeckCard_PropertyChanged;
    }
  }

  public ObservableCollection<DeckEditorMTGCard> DeckCards
  {
    get => field ??= DeckCards = [];
    set
    {
      if (field != null)
      {
        field.CollectionChanged -= DeckCards_CollectionChanged;

        foreach (var item in field)
          item.PropertyChanged -= DeckCard_PropertyChanged;
      }

      SetProperty(ref field, value);

      if (field != null)
      {
        field.CollectionChanged += DeckCards_CollectionChanged;

        foreach (var item in field)
          item.PropertyChanged += DeckCard_PropertyChanged;
      }
    }
  }
  public ObservableCollection<DeckEditorMTGCard> Wishlist
  {
    get => field ??= Wishlist = [];
    set
    {
      field?.CollectionChanged -= DeckCards_CollectionChanged;
      SetProperty(ref field, value);
      field?.CollectionChanged += DeckCards_CollectionChanged;
    }
  }
  public ObservableCollection<DeckEditorMTGCard> Maybelist
  {
    get => field ??= Maybelist = [];
    set
    {
      field?.CollectionChanged -= DeckCards_CollectionChanged;
      SetProperty(ref field, value);
      field?.CollectionChanged += DeckCards_CollectionChanged;
    }
  }
  public ObservableCollection<DeckEditorMTGCard> Removelist
  {
    get => field ??= Removelist = [];
    set
    {
      field?.CollectionChanged -= DeckCards_CollectionChanged;
      SetProperty(ref field, value);
      field?.CollectionChanged += DeckCards_CollectionChanged;
    }
  }

  public int DeckSize => DeckCards.Sum(x => x.Count) + (Commander != null ? 1 : 0) + (CommanderPartner != null ? 1 : 0);
  public double DeckPrice => DeckCards.Sum(x => x.Info.Price * x.Count) + (Commander?.Info.Price ?? 0) + (CommanderPartner?.Info.Price ?? 0);

  public event PropertyChangedEventHandler? PropertyChanged;
  public event PropertyChangingEventHandler? PropertyChanging;

  private void Deck_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(DeckCards):
      case nameof(CommanderPartner):
      case nameof(Commander):
        PropertyChanged?.Invoke(this, new(nameof(DeckSize)));
        PropertyChanged?.Invoke(this, new(nameof(DeckPrice)));
        break;
    }
  }

  private void DeckCards_CollectionChanged(object? _, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    foreach (var item in e.AddedItems<DeckEditorMTGCard>())
      item.PropertyChanged += DeckCard_PropertyChanged;
    foreach (var item in e.RemovedItems<DeckEditorMTGCard>())
      item.PropertyChanged -= DeckCard_PropertyChanged;

    PropertyChanged?.Invoke(this, new(nameof(DeckSize)));
    PropertyChanged?.Invoke(this, new(nameof(DeckPrice)));
  }

  private void DeckCard_PropertyChanged(object? _, PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(DeckEditorMTGCard.Info):
      case nameof(DeckEditorMTGCard.Count):
        PropertyChanged?.Invoke(this, new(nameof(DeckSize)));
        PropertyChanged?.Invoke(this, new(nameof(DeckPrice)));
        break;
    }
  }

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