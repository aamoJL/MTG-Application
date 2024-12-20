﻿using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.Features.DeckEditor.Editor.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.Charts.Models;

public partial class MTGCardChartSeriesItem : ObservableObject
{
  public MTGCardChartSeriesItem() => Cards.CollectionChanged += Cards_CollectionChanged;

  public ObservableCollection<DeckEditorMTGCard> Cards { get; } = [];

  public int Count => Cards.Sum(x => x.Count);

  protected void Cards_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    switch (e.Action)
    {
      case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
        if (e.NewItems?[0] is DeckEditorMTGCard newCard)
          newCard.PropertyChanged += CardsItem_PropertyChanged;
        break;
      case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
        if (e.OldItems?[0] is DeckEditorMTGCard oldCard)
          oldCard.PropertyChanged -= CardsItem_PropertyChanged;
        break;
    }

    OnPropertyChanged(nameof(Count));
  }

  protected void CardsItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(DeckEditorMTGCard.Count))
      OnPropertyChanged(nameof(Count));
  }
}