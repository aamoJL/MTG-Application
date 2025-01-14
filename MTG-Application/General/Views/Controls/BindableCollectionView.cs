using Microsoft.UI.Xaml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

  public ObservableCollection<object> View => CollectionView.View;

  private FilterableAndSortableCollectionView CollectionView { get; } = new();

  private void OnSourceChanged() => CollectionView.Source = Source;

  private void OnFilterChanged() => CollectionView.Filter = Filter;

  private void OnSortComparerChanged() => CollectionView.SortComparer = SortComparer;

  private static void OnDependencyPropertyChangedCallback(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    var collectionView = sender as BindableCollectionView;

    if (e.Property == SourceProperty)
      collectionView?.OnSourceChanged();
    else if (e.Property == FilterProperty)
      collectionView?.OnFilterChanged();
    else if (e.Property == SortComparerProperty)
      collectionView?.OnSortComparerChanged();
  }
}