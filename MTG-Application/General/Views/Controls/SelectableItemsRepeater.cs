using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System.Collections;

namespace MTGApplication.General.Views.Controls;

public partial class SelectableItemsRepeater : ItemsRepeater
{
  public static readonly DependencyProperty SelectionModeProperty =
      DependencyProperty.Register(nameof(SelectionMode), typeof(ListViewSelectionMode), typeof(SelectableItemsRepeater), new PropertyMetadata(ListViewSelectionMode.None));

  public SelectableItemsRepeater() : base()
  {
    ElementPrepared += OnElementPrepared;
    ElementClearing += OnElementClearing;
    LosingFocus += OnLosingFocus;

    PointerClick.Clicked += Item_Clicked;

    RegisterPropertyChangedCallback(ItemsSourceProperty, ItemsSourceOnPropertyChanged);
  }

  public ListViewSelectionMode SelectionMode
  {
    get => (ListViewSelectionMode)GetValue(SelectionModeProperty);
    set => SetValue(SelectionModeProperty, value);
  }
  public bool DeselectOnLosingFocus { get; set; } = false;

  public object? SelectedItem
  {
    get
    {
      if (SelectedElement == null)
        return null;

      var index = GetElementIndex(SelectedElement);

      if (index == -1)
        return null;

      return (ItemsSource as IList)?[index];
    }
  }

  protected UIElement? SelectedElement
  {
    get;
    set
    {
      if (field == value || SelectionMode == ListViewSelectionMode.None)
        return;

      if (TryGetContainer(field) is ItemContainer oldContainer)
        oldContainer.IsSelected = false;

      field = value;

      field?.Focus(FocusState.Programmatic);

      field?.StartBringIntoView(new()
      {
        AnimationDesired = true,
        VerticalAlignmentRatio = .5f,
      });

      if (TryGetContainer(field) is ItemContainer newContainer)
        newContainer.IsSelected = true;
    }
  }

  protected PointerClick PointerClick { get; } = new();

  public void DeselectAll() => SelectedElement = null;

  public void SelectItem(object item)
  {
    if (ItemsSourceView.IndexOf(item) is int index && index == -1)
      return;

    if (GetOrCreateElement(index) is UIElement element)
      SelectedElement = element;
  }

  private void OnElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
  {
    // Select the prepared item if it has focus
    //  For example, if the user redo remove command, the item will have focus
    if (args.Element.FindDescendantOrSelf<UIElement>(x => x.FocusState != FocusState.Unfocused) is not null)
      SelectedElement = args.Element;

    args.Element.GettingFocus += Item_GettingFocus;
    args.Element.GotFocus += Item_GotFocus;
    PointerClick.Register(args.Element);
  }

  private void OnElementClearing(ItemsRepeater sender, ItemsRepeaterElementClearingEventArgs args)
  {
    args.Element.GettingFocus -= Item_GettingFocus;
    args.Element.GotFocus -= Item_GotFocus;
    PointerClick.Unregister(args.Element);
  }

  private void OnLosingFocus(UIElement sender, LosingFocusEventArgs args)
  {
    if (DeselectOnLosingFocus)
    {
      if (args.NewFocusedElement is Popup)
        args.Handled = true; // New focused element is the selected item's flyout
      else if (args.NewFocusedElement is UIElement newElement && GetElementIndex(newElement) != -1)
        args.Handled = true; // New focused element is in this repeater
      else
        DeselectAll();
    }
  }

  private void ItemsSourceOnPropertyChanged(DependencyObject sender, DependencyProperty dp)
  {
    if (ItemsSourceView != null)
    {
      ItemsSourceView.CollectionChanged -= ItemsSourceView_CollectionChanged;
      ItemsSourceView.CollectionChanged += ItemsSourceView_CollectionChanged;
    }
  }

  private void ItemsSourceView_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldItems != null)
    {
      if (SelectedElement != null && GetElementIndex(SelectedElement) == -1)
      {
        var index = e.OldStartingIndex < ItemsSourceView.Count
          ? e.OldStartingIndex
          : e.OldStartingIndex - 1;

        if (index > -1 && (ItemsSource as IList)?[index] is object next)
          SelectItem(next);
        else
          DeselectAll();
      }
    }
  }

  private void Item_GettingFocus(UIElement sender, GettingFocusEventArgs args)
  {
    if (sender == SelectedElement)
      return;

    if (args.FocusState != FocusState.Keyboard)
      args.TryCancel();

    // Change selected item on focus only if the focus changed using keyboard navigation
    if (args.Direction
      is not (FocusNavigationDirection.Up
      or FocusNavigationDirection.Down
      or FocusNavigationDirection.Left
      or FocusNavigationDirection.Right))
      args.TryCancel();
  }

  private void Item_GotFocus(object sender, RoutedEventArgs e)
  {
    if (sender is not UIElement element)
      return;

    SelectedElement = element;
  }

  private void Item_Clicked(object? sender, PointerRoutedEventArgs e)
  {
    if (sender is not UIElement element)
      return;

    SelectedElement = element;
  }

  /// <summary>
  /// Returns ItemContainer from element's visualtree if possible
  /// </summary>
  private ItemContainer? TryGetContainer(UIElement? element)
    => element?.FindDescendantOrSelf<ItemContainer>();
}
