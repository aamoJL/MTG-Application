using CommunityToolkit.Mvvm.ComponentModel;
using MTGApplication.General.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.Features.DeckEditor.Models;

public partial class DeckEditorCardGroup : ObservableObject
{
  public DeckEditorCardGroup(string key, ObservableCollection<DeckEditorMTGCard> source)
  {
    PropertyChanged += Group_PropertyChanged;

    Source = source;
    GroupKey = key;

    foreach (var item in source)
      item.PropertyChanged += SourceItem_PropertyChanged;

    source.CollectionChanged += Source_CollectionChanged;
  }

  [ObservableProperty] public partial string GroupKey { get; set; }
  public ReadOnlyObservableCollection<DeckEditorMTGCard> Cards => field ??= new(GroupedCards);

  private ObservableCollection<DeckEditorMTGCard> GroupedCards { get; } = [];
  private ObservableCollection<DeckEditorMTGCard> Source { get; }

  public void AddToSource(DeckEditorMTGCard item) => Source.Add(item);

  public void RemoveFromSource(DeckEditorMTGCard item) => Source.Remove(item);

  public DeckEditorMTGCard? GetFromSource(Func<DeckEditorMTGCard, bool> predicate) => Source.FirstOrDefault(predicate);

  public bool SourceContains(DeckEditorMTGCard card) => Source.Contains(card);

  private void Group_PropertyChanged(object? _, System.ComponentModel.PropertyChangedEventArgs e)
  {
    switch (e.PropertyName)
    {
      case nameof(GroupKey):
        GroupedCards.Clear();
        foreach (var item in Source)
        {
          if (item.Group == GroupKey)
            GroupedCards.Add(item);
        }
        break;
    }
  }

  private void Source_CollectionChanged(object? _, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    foreach (var item in e.AddedItems<DeckEditorMTGCard>())
    {
      item.PropertyChanged += SourceItem_PropertyChanged;

      if (item.Group == GroupKey)
        GroupedCards.Add(item);
    }
    foreach (var item in e.RemovedItems<DeckEditorMTGCard>())
    {
      item.PropertyChanged -= SourceItem_PropertyChanged;

      GroupedCards.Remove(item);
    }
  }

  private void SourceItem_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (sender is not DeckEditorMTGCard card)
      return;

    switch (e.PropertyName)
    {
      case nameof(DeckEditorMTGCard.Group):
        if (card.Group == GroupKey)
        {
          if (!Cards.Contains(card))
            GroupedCards.Add(card);
        }
        else
          GroupedCards.Remove(card);
        break;
    }
  }
}