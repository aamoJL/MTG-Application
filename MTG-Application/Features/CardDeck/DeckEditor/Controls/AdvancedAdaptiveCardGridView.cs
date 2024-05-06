using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using MTGApplication.General.Models.Card;
using System.Collections;
using static MTGApplication.General.Models.Card.MTGCardSortProperties;

namespace MTGApplication.Features.CardDeck.DeckEditor.Controls;
public partial class AdvancedAdaptiveCardGridView : AdaptiveGridView
{
  public new static readonly DependencyProperty ItemsSourceProperty =
      DependencyProperty.Register(nameof(ItemsSource), typeof(IList), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(null, OnDependencyPropertyChanged));

  public static readonly DependencyProperty SortPropertiesProperty =
      DependencyProperty.Register(nameof(SortProperties), typeof(MTGCardSortProperties), typeof(AdvancedAdaptiveCardGridView), new PropertyMetadata(
        new MTGCardSortProperties(MTGSortProperty.CMC, MTGSortProperty.Name, SortDirection.Ascending), OnDependencyPropertyChanged));

  public AdvancedAdaptiveCardGridView()
  {
    var source = new AdvancedCollectionView();
    source.SortDescriptions.Add(new(SortProperties.SortDirection, new MTGCardComparer(SortProperties.PrimarySortProperty)));
    source.SortDescriptions.Add(new(SortProperties.SortDirection, new MTGCardComparer(SortProperties.SecondarySortProperty)));
    FilteredAndSortedCardSource = source;
  }

  public new IList ItemsSource
  {
    get => (IList)GetValue(ItemsSourceProperty);
    set => SetValue(ItemsSourceProperty, value);
  }

  public MTGCardSortProperties SortProperties
  {
    get => (MTGCardSortProperties)GetValue(SortPropertiesProperty);
    set => SetValue(SortPropertiesProperty, value);
  }

  private AdvancedCollectionView filteredAndSortedCardSource = new();
  private AdvancedCollectionView FilteredAndSortedCardSource
  {
    get => filteredAndSortedCardSource;
    set
    {
      filteredAndSortedCardSource = value;
      base.ItemsSource = filteredAndSortedCardSource;
    }
  }

  private void OnItemsSourceDependencyPropertyChanged(IList list)
  {
    if (list is null) return;

    FilteredAndSortedCardSource.Source = list;
  }

  private void OnSortPropertiesDependencyPropertyChanged(MTGCardSortProperties sortProperties)
  {
    if (sortProperties is null) return;

    FilteredAndSortedCardSource.SortDescriptions[0]
      = new(sortProperties.SortDirection, new MTGCardComparer(sortProperties.PrimarySortProperty));
    FilteredAndSortedCardSource.SortDescriptions[1]
      = new(sortProperties.SortDirection, new MTGCardComparer(sortProperties.SecondarySortProperty));
  }

  private static void OnDependencyPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
  {
    if (sender is not AdvancedAdaptiveCardGridView view) return;

    if (e.Property == ItemsSourceProperty)
      view.OnItemsSourceDependencyPropertyChanged(e.NewValue as IList);

    if (e.Property == SortPropertiesProperty)
      view.OnSortPropertiesDependencyPropertyChanged(e.NewValue as MTGCardSortProperties);
  }
}