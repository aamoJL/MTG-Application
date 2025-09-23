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
        if (field is INotifyCollectionChanged observable)
          foreach (var item in Source.OfType<INotifyPropertyChanged>())
            item.PropertyChanged -= SourceItem_PropertyChanged;

        field = value;

        View.Clear();

        field ??= Array.Empty<object>();

        foreach (var item in field)
        {
          AddToView(item, sort: false);

          if (item is INotifyPropertyChanged observableItem)
            observableItem.PropertyChanged += SourceItem_PropertyChanged;
        }

        Sort();

        SourceWeakEventListener?.Detach();

        if (field is INotifyCollectionChanged observableCollection)
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

  private bool AddToView(object item, bool sort = true)
  {
    if (View.Contains(item))
      return false;

    if (Filter == null || Filter(item))
    {
      if (sort && SortComparer != null)
        View.Insert(View.FindPosition(item, SortComparer), item);
      else
        View.Add(item);

      return true;
    }
    return false;
  }

  private bool RemoveFromView(object item)
  {
    try
    {
      return View.Remove(item);
    }
    catch { }

    return false;
  }

  private bool RemoveAtFromView(int index)
  {
    try
    {
      View.RemoveAt(index);
      return true;
    }
    catch { }

    return false;
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

    if (AddToView(item, sort: true))
      return;
    else if (Filter?.Invoke(item) == false)
    {
      RemoveFromView(item);
      return;
    }

    var index = View.IndexOf(item);

    if (index == -1)
      return;

    if (SortComparer != null)
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
