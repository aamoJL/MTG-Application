// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Specialized;
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

    _itemsSourceOnPropertyChangedToken = AssociatedObject.RegisterPropertyChangedCallback(ItemsRepeater.ItemsSourceProperty, ItemsSourceOnPropertyChanged);
  }

  /// <inheritdoc/>
  protected override void OnDetaching()
  {
    AssociatedObject.UnregisterPropertyChangedCallback(ItemsRepeater.ItemsSourceProperty, _itemsSourceOnPropertyChangedToken);

    _lastObservableCollection?.CollectionChanged -= TryRaiseLoadMoreRequested;
    ItemsRepeater?.SizeChanged -= TryRaiseLoadMoreRequested;
    ScrollViewer?.ViewChanged -= TryRaiseLoadMoreRequested;
  }

  private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
  {
    AssociatedObject.Loaded -= AssociatedObject_Loaded;

    if (ScrollViewer != null)
    {
      ScrollViewer.ViewChanged += TryRaiseLoadMoreRequested;
      ItemsRepeater?.SizeChanged += TryRaiseLoadMoreRequested;
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
        _lastObservableCollection?.CollectionChanged -= TryRaiseLoadMoreRequested;

        _lastObservableCollection = ncc;
        _lastObservableCollection?.CollectionChanged += TryRaiseLoadMoreRequested;
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

        if (LoadMoreRequested is not null && await LoadMoreRequested(AssociatedObject, EventArgs.Empty))
          ScrollViewer.UpdateLayout(); // updates scrollviewer size
        else
          loadMore = false;

        IsLoadingMore = false;
      }
      else
        loadMore = false;
    }
  }
}

