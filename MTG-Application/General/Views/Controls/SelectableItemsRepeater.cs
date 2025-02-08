using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections;

namespace MTGApplication.General.Views.Controls;

public partial class SelectableItemsRepeater : ItemsRepeater
{
  public static readonly DependencyProperty SelectionModeProperty =
      DependencyProperty.Register(nameof(SelectionMode), typeof(ListViewSelectionMode), typeof(SelectableItemsRepeater), new PropertyMetadata(ListViewSelectionMode.None));

  public SelectableItemsRepeater() : base()
  {
    ElementPrepared += AdvancedItemsRepeater_ElementPrepared;
    ElementClearing += AdvancedItemsRepeater_ElementClearing;
    LosingFocus += SelectableItemsRepeater_LosingFocus;

    PointerClick.Clicked += Item_Clicked;
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

    if (TryGetElement(index) is UIElement element)
      element.Focus(FocusState.Programmatic);
  }

  private void AdvancedItemsRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
  {
    // Select the prepared item if it has focus
    //  For example, if the user redo remove command, the item will have focus
    if (args.Element.FindDescendant<UIElement>(x => x.FocusState != FocusState.Unfocused) is not null)
      SelectedElement = args.Element;

    args.Element.GotFocus += Item_GotFocus;
    PointerClick.Register(args.Element);
  }

  private void AdvancedItemsRepeater_ElementClearing(ItemsRepeater sender, ItemsRepeaterElementClearingEventArgs args)
  {
    if (args.Element == SelectedElement)
    {
      // TODO: select next item
      DeselectAll();
    }

    args.Element.GotFocus -= Item_GotFocus;
    PointerClick.Unregister(args.Element);
  }

  private void Item_GotFocus(object sender, RoutedEventArgs e)
  {
    if (sender is not UIElement element)
      return;

    if (e.OriginalSource is UIElement { FocusState: FocusState.Pointer or FocusState.Keyboard })
    {
      element.StartBringIntoView(new()
      {
        AnimationDesired = true,
        VerticalAlignmentRatio = .5f,
      });
    }

    SelectedElement = element;
  }

  private void Item_Clicked(object? sender, PointerRoutedEventArgs e)
    => (sender as UIElement)?.Focus(FocusState.Pointer);

  private void SelectableItemsRepeater_LosingFocus(UIElement sender, LosingFocusEventArgs args)
  {
    if (DeselectOnLosingFocus)
      DeselectAll();
  }

  /// <summary>
  /// Returns ItemContainer from element's visualtree if possible
  /// </summary>
  private ItemContainer? TryGetContainer(UIElement? element)
    => element?.FindDescendantOrSelf<ItemContainer>();
}
