using CommunityToolkit.WinUI.Helpers;
using MTGApplication.General.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MTGApplication.General.Views.Controls;

public class FilterableAndSortableCollectionView<T>
{
  public FilterableAndSortableCollectionView(IList<T> source, IValueFilter<T> filters, IValueSorter<T> sorter)
  {
    Filter = filters;
    Sorter = sorter;
    Source = source;

    ApplySort();
    ApplyFilter();
  }

  public IList<T> Source
  {
    get;
    private init
    {
      field = value;

      foreach (var item in field)
      {
        AddToView(item, sort: false);

        if (item is INotifyPropertyChanged observableItem)
          observableItem.PropertyChanged += SourceItem_PropertyChanged;
      }

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
  public IValueFilter<T> Filter
  {
    get;
    private init
    {
      field = value;
      field.PropertyChanged += Filter_PropertyChanged;
    }
  }
  public IValueSorter<T> Sorter
  {
    get;
    private init
    {
      field = value;
      field.PropertyChanged += Sorter_PropertyChanged;
    }
  }

  public ObservableCollection<T> View { get; } = [];

  private WeakEventListener<FilterableAndSortableCollectionView<T>, object, NotifyCollectionChangedEventArgs>? SourceWeakEventListener { get; set; }

  private void ApplyFilter()
  {
    for (var i = 0; i < View.Count; i++)
    {
      var item = View[i];

      if (!Filter.ValidationPredicate(item))
      {
        RemoveAtFromView(i);
        i--; // item at the current position was removed
      }
    }

    foreach (var item in Source)
      AddToView(item);
  }

  private void ApplySort()
  {
    var temp = View.ToList();
    temp.Sort(Sorter.Comparer);

    View.Clear();

    foreach (var item in temp)
      View.Add(item);
  }

  private bool AddToView(T item, bool sort = true)
  {
    if (View.Contains(item))
      return false;

    if (Filter.ValidationPredicate(item))
    {
      if (sort && Sorter != null)
        View.Insert(View.FindPosition(item, Sorter.Comparer), item);
      else
        View.Add(item);

      return true;
    }
    return false;
  }

  private bool RemoveFromView(T item)
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
    foreach (var item in e.AddedItems<T>())
    {
      if (item is INotifyPropertyChanged observableItem)
        observableItem.PropertyChanged += SourceItem_PropertyChanged;

      AddToView(item);
    }
    foreach (var item in e.RemovedItems<T>())
    {
      if (item is INotifyPropertyChanged observableItem)
        observableItem.PropertyChanged -= SourceItem_PropertyChanged;

      RemoveFromView(item);
    }

    if (e.Action == NotifyCollectionChangedAction.Reset)
      View.Clear();
  }

  private void SourceItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (sender is not T item) return;
    if (!Source.Contains(item)) return;

    if (AddToView(item, sort: true)) return;

    if (!Filter.ValidationPredicate(item))
    {
      RemoveFromView(item);
      return;
    }

    var index = View.IndexOf(item);

    if (index == -1)
      return;

    if (Sorter != null)
    {
      if ((index - 1 >= 0 && Sorter.Comparer.Compare(item, View[index - 1]) < 0))
      {
        var tempList = View.Take(new Range(0, index));
        var newIndex = tempList.FindPosition(item, Sorter.Comparer);

        View.Move(index, newIndex);
      }
      else if ((index + 1 < View.Count && Sorter.Comparer.Compare(item, View[index + 1]) > 0))
      {
        var tempList = View.Take(new Range(index + 1, View.Count));
        var newIndex = index + tempList.FindPosition(item, Sorter.Comparer);

        View.Move(index, newIndex);
      }
    }
  }

  private void Sorter_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(Sorter.Comparer))
      ApplySort();
  }

  private void Filter_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(Filter.ValidationPredicate))
      ApplyFilter();
  }
}

public interface IValueFilter<T> : INotifyPropertyChanged
{
  public Predicate<T> ValidationPredicate { get; }
}

public interface IValueSorter<T> : INotifyPropertyChanged
{
  public IComparer<T> Comparer { get; }
}
