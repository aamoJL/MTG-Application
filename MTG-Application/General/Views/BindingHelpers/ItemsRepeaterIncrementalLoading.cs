// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace MTGApplication.General.Views.BindingHelpers;

/// <summary>
/// A behavior that makes <see cref="ItemsRepeater"/> support <see cref="ISupportIncrementalLoading"/>.
/// </summary>
public class ItemsRepeaterIncrementalLoading : Behavior<ItemsRepeater>
{
  /// <summary>
  /// Identifies the <see cref="LoadingOffset"/> property.
  /// </summary>
  public static readonly DependencyProperty LoadingOffsetProperty =
    DependencyProperty.Register(nameof(LoadingOffset), typeof(double), typeof(ItemsRepeaterIncrementalLoading), new PropertyMetadata(100d));

  /// <summary>
  /// Identifies the <see cref="IsActive"/> property.
  /// </summary>
  public static readonly DependencyProperty IsActiveProperty =
    DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(ItemsRepeaterIncrementalLoading), new PropertyMetadata(true));

  /// <summary>
  /// Identifies the <see cref="IsLoadingMore"/> property.
  /// </summary>
  public static readonly DependencyProperty IsLoadingMoreProperty =
    DependencyProperty.Register(nameof(IsLoadingMore), typeof(bool), typeof(ItemsRepeaterIncrementalLoading), new PropertyMetadata(false));

  /// <summary>
  /// Identifies the <see cref="LoadCount"/> property.
  /// </summary>
  public static readonly DependencyProperty LoadCountProperty =
    DependencyProperty.Register(nameof(LoadCount), typeof(int), typeof(ItemsRepeaterIncrementalLoading), new PropertyMetadata(20));

  /// <summary>
  /// Gets or sets Distance of content from scrolling to bottom.
  /// </summary>
  public double LoadingOffset
  {
    get => (double)GetValue(LoadingOffsetProperty);
    set => SetValue(LoadingOffsetProperty, value);
  }

  /// <summary>
  /// Gets a value indicating whether the behavior is active.
  /// </summary>
  public bool IsActive
  {
    get => (bool)GetValue(IsActiveProperty);
    set => SetValue(IsActiveProperty, value);
  }

  /// <summary>
  /// Gets or sets if more items are being loaded.
  /// </summary>
  public bool IsLoadingMore
  {
    get => (bool)GetValue(IsLoadingMoreProperty);
    set => SetValue(IsLoadingMoreProperty, value);
  }

  /// <summary>
  /// Gets or sets the "count" parameter when triggering <see cref="ISupportIncrementalLoading.LoadMoreItemsAsync"/>.
  /// </summary>
  public int LoadCount
  {
    get => (int)GetValue(LoadCountProperty);
    set => SetValue(LoadCountProperty, value);
  }

  /// <summary>
  /// Raised when more items need to be loaded.
  /// </summary>
  public event Func<ItemsRepeater, EventArgs, Task<bool>>? LoadMoreRequested;

  private ItemsRepeater? ItemsRepeater => AssociatedObject;
  private ScrollViewer? ScrollViewer => field ??= AssociatedObject.FindAscendant<ScrollViewer>();

  private long _itemsSourceOnPropertyChangedToken;

  private INotifyCollectionChanged? _lastObservableCollection;

  /// <inheritdoc/>
  protected override void OnAttached()
  {
    LoadMoreRequested += async (sender, args) =>
    {
      if (sender.ItemsSource is ISupportIncrementalLoading sil)
      {
        _ = await sil.LoadMoreItemsAsync((uint)LoadCount);
        return sil.HasMoreItems;
      }

      return false;
    };

    AssociatedObject.Loaded += AssociatedObject_Loaded;

    _itemsSourceOnPropertyChangedToken = RegisterPropertyChangedCallback(ItemsRepeater.ItemsSourceProperty, ItemsSourceOnPropertyChanged);
  }

  /// <inheritdoc/>
  protected override void OnDetaching()
  {
    AssociatedObject.UnregisterPropertyChangedCallback(ItemsRepeater.ItemsSourceProperty, _itemsSourceOnPropertyChangedToken);

    if (_lastObservableCollection is not null)
      _lastObservableCollection.CollectionChanged -= TryRaiseLoadMoreRequested;
    if (ItemsRepeater is not null)
      ItemsRepeater.SizeChanged -= TryRaiseLoadMoreRequested;
    if (ScrollViewer is not null)
      ScrollViewer.ViewChanged -= TryRaiseLoadMoreRequested;
  }

  private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
  {
    AssociatedObject.Loaded -= AssociatedObject_Loaded;

    if (ScrollViewer != null)
    {
      ScrollViewer.ViewChanged += TryRaiseLoadMoreRequested;

      if (ItemsRepeater is not null)
        ItemsRepeater.SizeChanged += TryRaiseLoadMoreRequested;
    }
  }

  /// <summary>
  /// When the data source changes or <see cref="NotifyCollectionChangedAction.Reset"/>, <see cref="NotifyCollectionChangedAction.Remove"/>.
  /// This method reloads the data.
  /// This method is intended to solve the problem of reloading data when the data source changes and the <see cref="ItemsRepeater"/>'s <see cref="FrameworkElement.ActualHeight"/> does not change
  /// </summary>
  private async void ItemsSourceOnPropertyChanged(DependencyObject sender, DependencyProperty dp)
  {
    if (sender is ItemsRepeater { ItemsSource: ISupportIncrementalLoading sil })
    {
      if (sil is INotifyCollectionChanged ncc)
      {
        if (_lastObservableCollection is not null)
          _lastObservableCollection.CollectionChanged -= TryRaiseLoadMoreRequested;

        _lastObservableCollection = ncc;
        ncc.CollectionChanged += TryRaiseLoadMoreRequested;
      }

      // On the first load, the `ScrollViewer` is not yet initialized.
      if (ScrollViewer is not null)
        await TryRaiseLoadMoreRequestedAsync();
    }
  }

  private async void TryRaiseLoadMoreRequested(object? sender, object e)
    => await TryRaiseLoadMoreRequestedAsync();

  /// <summary>
  /// Determines if the scroll view has scrolled to the bottom, and if so triggers the <see cref="LoadMoreRequested"/>.
  /// This event will only cause the source to load at most once
  /// </summary>
  public async Task TryRaiseLoadMoreRequestedAsync()
  {
    if (AssociatedObject is null || ScrollViewer is null)
      return;

    var loadMore = true;

    // Load until a new item is loaded in
    while (loadMore)
    {
      if (AssociatedObject is null || !IsActive || IsLoadingMore)
        return;

      // LoadMoreRequested is only triggered when the view is not filled.
      if ((ScrollViewer.ScrollableHeight is 0 && ScrollViewer.ScrollableWidth is 0) ||
          (ScrollViewer.ScrollableHeight > 0 &&
           ScrollViewer.ScrollableHeight - LoadingOffset < ScrollViewer.VerticalOffset) ||
          (ScrollViewer.ScrollableWidth > 0 &&
           ScrollViewer.ScrollableWidth - LoadingOffset < ScrollViewer.HorizontalOffset))
      {
        IsLoadingMore = true;

        var before = GetItemsCount();

        if (LoadMoreRequested is not null && await LoadMoreRequested(AssociatedObject, EventArgs.Empty))
        {
          var after = GetItemsCount();

          // This can be set to the count of items in a row,
          // so that it can continue to load even if the count of items loaded is too small.
          // Generally, 20 items will be loaded at a time,
          // and the count of items in a row is usually less than 10, so it is set to 10 here.
          if (before + 10 <= after)
            loadMore = false;
        }
        else
          // No more items or ItemsSource is null
          loadMore = false;

        IsLoadingMore = false;
      }
      else
      {
        // There is no need to continue loading if it fills up the view
        loadMore = false;
      }
    }
  }

  private int GetItemsCount()
  {
    return AssociatedObject?.ItemsSource switch
    {
      ICollection list => list.Count,
      IEnumerable enumerable => enumerable.Cast<object>().Count(),
      null => 0,
      _ => throw new ArgumentOutOfRangeException(nameof(AssociatedObject.ItemsSource))
    };
  }
}

