using CommunityToolkit.WinUI.Helpers;
using MTGApplication.General.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MTGApplication.General.Views.Controls;

public class FilterableAndSortableCollectionView
{
  public IList Source
  {
    get;
    set
    {
      if (field != value)
      {
        if (Source is INotifyCollectionChanged observable)
          foreach (var item in Source.OfType<INotifyPropertyChanged>())
            item.PropertyChanged -= SourceItem_PropertyChanged;

        field = value;

        View.Clear();

        Source ??= Array.Empty<object>();

        foreach (var item in Source)
        {
          AddToView(item, sort: false);

          if (item is INotifyPropertyChanged observableItem)
            observableItem.PropertyChanged += SourceItem_PropertyChanged;
        }

        Sort();

        SourceWeakEventListener?.Detach();

        if (Source is INotifyCollectionChanged observableCollection)
        {
          SourceWeakEventListener = new(this)
          {
            OnEventAction = (sender, _, e) => Source_CollectionChanged(sender, e),
            OnDetachAction = (listener) => observableCollection.CollectionChanged -= SourceWeakEventListener!.OnEvent!
          };

          observableCollection.CollectionChanged += SourceWeakEventListener.OnEvent!;
        }
      }
    }
  } = new List<object>();
  public Predicate<object>? Filter
  {
    get;
    set
    {
      field = value;
      OnFilterChanged();
    }
  }
  public IComparer<object>? SortComparer
  {
    get;
    set
    {
      if (value != field)
      {
        field = value;
        OnSortComparerChanged();
      }
    }
  }

  public ObservableCollection<object> View { get; } = [];

  private WeakEventListener<FilterableAndSortableCollectionView, object, NotifyCollectionChangedEventArgs>? SourceWeakEventListener { get; set; }

  private void OnSortComparerChanged() => Sort();

  private void OnFilterChanged()
  {
    for (var i = 0; i < View.Count; i++)
    {
      var item = View[i];

      if (Filter != null && !Filter(item))
      {
        RemoveAtFromView(i);
        i--; // item at the current position was removed
      }
    }

    foreach (var item in Source)
      if (!View.Contains(item))
        AddToView(item);
  }

  private void Sort()
  {
    if (SortComparer == null)
      return;

    var temp = View.ToList();
    temp.Sort(SortComparer);

    View.Clear();

    foreach (var item in temp)
      View.Add(item);
  }

  private void AddToView(object item, bool sort = true)
  {
    if (Filter == null || Filter(item))
    {
      if (SortComparer != null && sort)
        View.Insert(View.FindPosition(item, SortComparer), item);
      else
        View.Add(item);
    }
  }

  private void RemoveFromView(object item)
  {
    try
    {
      View.Remove(item);
    }
    catch { }
  }

  private void RemoveAtFromView(int index)
  {
    if (index >= 0 && index < View.Count)
      View.RemoveAt(index);
  }

  private void Source_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
    {
      foreach (var item in e.NewItems)
      {
        if (item is INotifyPropertyChanged observableItem)
          observableItem.PropertyChanged += SourceItem_PropertyChanged;

        AddToView(item);
      }
    }
    else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
    {
      foreach (var item in e.OldItems)
      {
        if (item is INotifyPropertyChanged observableItem)
          observableItem.PropertyChanged -= SourceItem_PropertyChanged;

        RemoveFromView(item);
      }
    }
    else if (e.Action == NotifyCollectionChangedAction.Reset)
      View.Clear();
  }

  private void SourceItem_PropertyChanged(object? item, PropertyChangedEventArgs e)
  {
    if (item == null || !Source.Contains(item))
      return;

    var sorted = false;
    var index = View.IndexOf(item);

    if (Filter != null)
    {
      if (Filter(item) && index == -1)
      {
        AddToView(item);
        sorted = true;
      }
      else if (!Filter(item) && index != -1)
      {
        RemoveAtFromView(index);
        sorted = true;
      }
    }

    if (SortComparer != null && !sorted)
    {
      if ((index - 1 >= 0 && SortComparer.Compare(item, View[index - 1]) < 0))
      {
        var tempList = View.Take(new Range(0, index));
        var newIndex = tempList.FindPosition(item, SortComparer);

        View.Move(index, newIndex);
      }
      else if ((index + 1 < View.Count && SortComparer.Compare(item, View[index + 1]) > 0))
      {
        var tempList = View.Take(new Range(index + 1, View.Count));
        var newIndex = index + tempList.FindPosition(item, SortComparer);

        View.Move(index, newIndex);
      }
    }
  }
}
