using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Views;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;
using static MTGApplication.General.Models.Card.CardSortProperties;

namespace MTGApplication.Features.DeckEditor;
public partial class AdvancedAdaptiveCardGridView : AdaptiveGridView
{
  public new static readonly DependencyProperty ItemsSourceProperty =
      DependencyProperty.Register(nameof(ItemsSource), typeof(IList<MTGCard>), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(null, OnDependencyPropertyChanged));

  public static readonly DependencyProperty SortPropertiesProperty =
      DependencyProperty.Register(nameof(SortProperties), typeof(CardSortProperties), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(
        new CardSortProperties(MTGSortProperty.CMC, MTGSortProperty.Name, SortDirection.Ascending), OnDependencyPropertyChanged));

  public static readonly DependencyProperty FilterPropertiesProperty =
      DependencyProperty.Register(nameof(FilterProperties), typeof(CardFilters), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(
        new CardFilters(), OnDependencyPropertyChanged));

  public AdvancedAdaptiveCardGridView()
  {
    DragAndDrop = new(new MTGCardCopier())
    {
      OnCopy = async (item) => await OnDropCopy?.ExecuteAsync(item),
      OnRemove = (item) => OnDropRemove?.Execute(item),
      OnExternalImport = async (data) => await OnDropImport?.ExecuteAsync(data),
      OnBeginMoveTo = async (item) => await OnDropBeginMoveTo?.ExecuteAsync(item),
      OnBeginMoveFrom = (item) => OnDropBeginMoveFrom?.Execute(item),
      OnExecuteMove = (item) => OnDropExecuteMove?.Execute(item)
    };

    DragItemsStarting += DragAndDrop.DragStarting;
    DragItemsCompleted += DragAndDrop.DragCompleted;
    DragOver += DragAndDrop.DragOver;
    Drop += DragAndDrop.Drop;
  }

  private AdvancedCollectionView filteredAndSortedCardSource = new();

  public new IList<MTGCard> ItemsSource
  {
    get => (IList<MTGCard>)GetValue(ItemsSourceProperty);
    set => SetValue(ItemsSourceProperty, value);
  }

  public CardSortProperties SortProperties
  {
    get => (CardSortProperties)GetValue(SortPropertiesProperty);
    set => SetValue(SortPropertiesProperty, value);
  }

  public CardFilters FilterProperties
  {
    get => (CardFilters)GetValue(FilterPropertiesProperty);
    set => SetValue(FilterPropertiesProperty, value);
  }

  private ListViewDragAndDrop DragAndDrop { get; }
  private AdvancedCollectionView FilteredAndSortedCardSource
  {
    get => filteredAndSortedCardSource;
    set
    {
      filteredAndSortedCardSource = value;
      base.ItemsSource = filteredAndSortedCardSource;
    }
  }

  public IAsyncRelayCommand OnDropCopy { get; set; }
  public ICommand OnDropRemove { get; set; }
  public IAsyncRelayCommand OnDropImport { get; set; }
  public ICommand OnDropBeginMoveFrom { get; set; }
  public IAsyncRelayCommand OnDropBeginMoveTo { get; set; }
  public ICommand OnDropExecuteMove { get; set; }

  private void OnItemsSourceDependencyPropertyChanged(IList list)
  {
    if (list is null) return;

    var source = new AdvancedCollectionView(list, true);
    source.SortDescriptions.Add(new(SortProperties.SortDirection, new MTGCardPropertyComparer(SortProperties.PrimarySortProperty)));
    source.SortDescriptions.Add(new(SortProperties.SortDirection, new MTGCardPropertyComparer(SortProperties.SecondarySortProperty)));
    FilteredAndSortedCardSource = source;
  }

  private void OnSortPropertiesDependencyPropertyChanged(CardSortProperties sortProperties)
  {
    if (sortProperties is null || FilteredAndSortedCardSource.SortDescriptions.Count == 0) return;

    FilteredAndSortedCardSource.SortDescriptions[0]
      = new(sortProperties.SortDirection, new MTGCardPropertyComparer(sortProperties.PrimarySortProperty));
    FilteredAndSortedCardSource.SortDescriptions[1]
      = new(sortProperties.SortDirection, new MTGCardPropertyComparer(sortProperties.SecondarySortProperty));
  }

  private void OnFilterPropertiesDependencyPropertyChanged(CardFilters filterProperties)
  {
    if (filterProperties is not null)
      filterProperties.PropertyChanged += FilterProperties_PropertyChanged;
  }

  private void FilterProperties_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (FilterProperties.FiltersApplied)
    {
      FilteredAndSortedCardSource.Filter = x => FilterProperties.CardValidation(x as MTGCard);
      FilteredAndSortedCardSource.RefreshFilter();
    }
    else { FilteredAndSortedCardSource.Filter = null; }
  }

  private static void OnDependencyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    if (sender is not AdvancedAdaptiveCardGridView view) return;

    if (e.Property == ItemsSourceProperty)
      view.OnItemsSourceDependencyPropertyChanged(e.NewValue as IList);

    if (e.Property == SortPropertiesProperty)
      view.OnSortPropertiesDependencyPropertyChanged(e.NewValue as CardSortProperties);

    if (e.Property == FilterPropertiesProperty)
      view.OnFilterPropertiesDependencyPropertyChanged(e.NewValue as CardFilters);
  }
}