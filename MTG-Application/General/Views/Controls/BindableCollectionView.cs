using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using MTGApplication.General.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace MTGApplication.General.Views.Controls;

public partial class BindableCollectionView : DependencyObject
{
  public static readonly DependencyProperty SourceProperty =
      DependencyProperty.Register(nameof(Source), typeof(IList), typeof(BindableCollectionView), new PropertyMetadata(Array.Empty<object>(), OnDependencyPropertyChangedCallback));

  public static readonly DependencyProperty FilterProperty =
      DependencyProperty.Register(nameof(Filter), typeof(Predicate<object>), typeof(BindableCollectionView), new PropertyMetadata(null, OnDependencyPropertyChangedCallback));

  public static readonly DependencyProperty SortComparerProperty =
      DependencyProperty.Register(nameof(SortComparer), typeof(IComparer<object>), typeof(BindableCollectionView), new PropertyMetadata(null, OnDependencyPropertyChangedCallback));

  public IList Source
  {
    get => (IList)GetValue(SourceProperty);
    set => SetValue(SourceProperty, value);
  }
  public Predicate<object>? Filter
  {
    get => (Predicate<object>)GetValue(FilterProperty);
    set => SetValue(FilterProperty, value);
  }
  public IComparer<object>? SortComparer
  {
    get => (IComparer<object>)GetValue(SortComparerProperty);
    set => SetValue(SortComparerProperty, value);
  }

  public ObservableCollection<object> View { get; set; } = [];

  private WeakEventListener<BindableCollectionView, object, NotifyCollectionChangedEventArgs>? SourceWeakEventListener { get; set; }

  private void Sort()
  {
    if (SortComparer == null)
      return;

    var temp = new List<object>(View);
    temp.Sort(SortComparer);

    View.Clear();

    foreach (var item in temp)
      View.Add(item);
  }

  private void OnSourceChanging()
  {
    if (Source is INotifyCollectionChanged observable)
    {
      SourceWeakEventListener?.Detach();

      foreach (var item in Source.OfType<INotifyPropertyChanged>())
        item.PropertyChanged -= SourceItem_PropertyChanged;
    }
  }

  private void OnSourceChanged()
  {
    View.Clear();

    Source ??= Array.Empty<object>();

    foreach (var item in Source)
      AddToView(item, sort: false);

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

  private void OnFilterChanged()
  {
    for (var i = 0; i < View.Count; i++)
    {
      var item = View[i];

      if (Filter != null && !Filter(item))
      {
        RemoveFromView(i);
        i--; // item at the current position was removed
      }
    }

    foreach (var item in Source)
      if (!View.Contains(item))
        AddToView(item);
  }

  private void OnSortDescriptionsChanged() => Sort();

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

  private void SourceItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (Filter == null || sender == null)
      return;

    if (!View.Contains(sender))
      AddToView(sender);
    else
      RemoveFromView(sender);
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

  private void RemoveFromView(object item) => View.Remove(item);

  private void RemoveFromView(int index) => View.RemoveAt(index);

  private static void OnDependencyPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    var collectionView = sender as BindableCollectionView;

    if (e.Property == SourceProperty)
    {
      collectionView?.OnSourceChanging();
      collectionView?.OnSourceChanged();
    }
    else if (e.Property == FilterProperty)
      collectionView?.OnFilterChanged();
    else if (e.Property == SortComparerProperty)
      collectionView?.OnSortDescriptionsChanged();
  }
}
