using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using MTGApplication.Features.DeckEditor.CardList.Services;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using static MTGApplication.Features.DeckEditor.CardList.Services.CardSortProperties;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardListView;
public partial class AdvancedAdaptiveCardGridView : AdaptiveGridView
{
  public new static readonly DependencyProperty ItemsSourceProperty =
      DependencyProperty.Register(nameof(ItemsSource), typeof(IList<DeckEditorMTGCard>), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(null, OnDependencyPropertyChanged));

  public static readonly DependencyProperty SortPropertiesProperty =
      DependencyProperty.Register(nameof(SortProperties), typeof(CardSortProperties), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(
        new CardSortProperties(MTGSortProperty.CMC, MTGSortProperty.Name, SortDirection.Ascending), OnDependencyPropertyChanged));

  public static readonly DependencyProperty FilterPropertiesProperty =
      DependencyProperty.Register(nameof(FilterProperties), typeof(CardFilters), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(
        new CardFilters(), OnDependencyPropertyChanged));

  public static readonly DependencyProperty OnDropCopyProperty =
      DependencyProperty.Register(nameof(OnDropCopy), typeof(IAsyncRelayCommand), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropImportProperty =
      DependencyProperty.Register(nameof(OnDropImport), typeof(IAsyncRelayCommand), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveFromProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveFrom), typeof(IRelayCommand), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropBeginMoveToProperty =
      DependencyProperty.Register(nameof(OnDropBeginMoveTo), typeof(IAsyncRelayCommand), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(default));

  public static readonly DependencyProperty OnDropExecuteMoveProperty =
      DependencyProperty.Register(nameof(OnDropExecuteMove), typeof(IRelayCommand), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(default));

  public AdvancedAdaptiveCardGridView()
  {
    DragAndDrop = new(itemToArgsConverter: (item) => new CardMoveArgs(item, item.Count))
    {
      OnCopy = async (item) => await (OnDropCopy?.ExecuteAsync(new DeckEditorMTGCard(item.Card.Info, item.Count)) ?? Task.CompletedTask),
      OnExternalImport = async (data) => await (OnDropImport?.ExecuteAsync(data) ?? Task.CompletedTask),
      OnBeginMoveTo = async (item) => await (OnDropBeginMoveTo?.ExecuteAsync((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count)) ?? Task.CompletedTask),
      OnBeginMoveFrom = (item) => OnDropBeginMoveFrom?.Execute((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count)),
      OnExecuteMove = (item) => OnDropExecuteMove?.Execute((item.Card as DeckEditorMTGCard) ?? new DeckEditorMTGCard(item.Card.Info, item.Count))
    };

    DragItemsStarting += DragAndDrop.DragStarting;
    DragItemsCompleted += DragAndDrop.DragCompleted;
  }

  // Needs to be overridden with new,
  //  otherwise the filtering and sorting bindings do not work on the ItemSource
  public new IList<DeckEditorMTGCard> ItemsSource
  {
    get => (IList<DeckEditorMTGCard>)GetValue(ItemsSourceProperty);
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

  private AdvancedCollectionView filteredAndSortedCardSource = [];
  protected ListViewDragAndDrop<DeckEditorMTGCard> DragAndDrop { get; }
  private AdvancedCollectionView FilteredAndSortedCardSource
  {
    get => filteredAndSortedCardSource;
    set
    {
      filteredAndSortedCardSource = value;
      base.ItemsSource = filteredAndSortedCardSource;
    }
  }

  public IAsyncRelayCommand OnDropCopy
  {
    get => (IAsyncRelayCommand)GetValue(OnDropCopyProperty);
    set => SetValue(OnDropCopyProperty, value);
  }
  public IAsyncRelayCommand OnDropImport
  {
    get => (IAsyncRelayCommand)GetValue(OnDropImportProperty);
    set => SetValue(OnDropImportProperty, value);
  }
  public IRelayCommand OnDropBeginMoveFrom
  {
    get => (IRelayCommand)GetValue(OnDropBeginMoveFromProperty);
    set => SetValue(OnDropBeginMoveFromProperty, value);
  }
  public IAsyncRelayCommand OnDropBeginMoveTo
  {
    get => (IAsyncRelayCommand)GetValue(OnDropBeginMoveToProperty);
    set => SetValue(OnDropBeginMoveToProperty, value);
  }
  public IRelayCommand OnDropExecuteMove
  {
    get => (IRelayCommand)GetValue(OnDropExecuteMoveProperty);
    set => SetValue(OnDropExecuteMoveProperty, value);
  }

  private void OnItemsSourceDependencyPropertyChanged(IList list)
  {
    if (list is null)
      return;

    var source = new AdvancedCollectionView(list, true);
    source.SortDescriptions.Add(new(SortProperties.SortDirection, new MTGCardPropertyComparer(SortProperties.PrimarySortProperty)));
    source.SortDescriptions.Add(new(SortProperties.SortDirection, new MTGCardPropertyComparer(SortProperties.SecondarySortProperty)));
    FilteredAndSortedCardSource = source;
  }

  private void OnSortPropertiesDependencyPropertyChanged(CardSortProperties sortProperties)
  {
    if (sortProperties is null || FilteredAndSortedCardSource.SortDescriptions.Count == 0)
      return;

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
      FilteredAndSortedCardSource.Filter = x => FilterProperties.CardValidation(x as DeckEditorMTGCard);
      FilteredAndSortedCardSource.RefreshFilter();
    }
    else
      FilteredAndSortedCardSource.Filter = null;
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

  protected override void OnDrop(DragEventArgs e) => DragAndDrop.Drop(e);

  protected override void OnDragOver(DragEventArgs e) => DragAndDrop.DragOver(e);
}