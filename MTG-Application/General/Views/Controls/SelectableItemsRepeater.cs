using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using System;
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
    ElementIndexChanged += OnElementIndexChanged;
    LosingFocus += OnLosingFocus;

    PointerClick.Clicked += Item_Clicked;
  }

  public ListViewSelectionMode SelectionMode
  {
    get => (ListViewSelectionMode)GetValue(SelectionModeProperty);
    set => SetValue(SelectionModeProperty, value);
  }
  public bool DeselectOnLosingFocus { get; set; } = false;
  public bool CenterOnFocus { get; set; } = false;
  public bool CanDragItems { get; set; } = false;

  public object? SelectedItem => SelectedIndex >= 0 ? ItemsSourceView.GetAt(SelectedIndex) : null;
  public int SelectedIndex => SelectedElement?.SelectionIndex ?? -1;
  public ISelectable? SelectedElement
  {
    get;
    private set
    {
      if (field == value) return;

      if (field is ISelectable oldElement)
        oldElement.IsSelected = false;

      field = value;

      if (field is ISelectable newElement)
        newElement.IsSelected = true;
    }
  } = null;

  // Tapped event will cause the element to lose focus,
  //  so the item tapping needs to be done with pointer press events
  // TODO: try tapped event with the refactored code
  protected PointerClick PointerClick { get; } = new();

  public event EventHandler<DragStartingEventArgs>? DragItemsStarting;

  private void OnElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
  {
    if (args.Element is ISelectable selectable)
    {
      selectable.SelectionIndex = args.Index;

      PointerClick.Register(args.Element);

      args.Element.GettingFocus += Item_GettingFocus;

      if (selectable.SelectionIndex == SelectedIndex)
        SelectedElement = selectable;
    }

    if (CanDragItems)
    {
      args.Element.DragStarting += Item_DragStarting;
      args.Element.CanDrag = true;
    }
  }

  private void OnElementIndexChanged(ItemsRepeater sender, ItemsRepeaterElementIndexChangedEventArgs args)
  {
    if (args.Element is ISelectable selectable)
    {
      selectable.SelectionIndex = args.NewIndex;

      if (selectable.SelectionIndex == SelectedIndex)
      {
        SelectedElement = selectable;

        // Deleting element using a flyout will not focus the next element, so the focus needs to be changed here
        if (selectable is UIElement { FocusState: FocusState.Unfocused } element)
          element.Focus(FocusState.Programmatic);
      }
    }
  }

  private void OnElementClearing(ItemsRepeater sender, ItemsRepeaterElementClearingEventArgs args)
  {
    if (args.Element is ISelectable selectable)
    {
      if (SelectedElement == selectable)
      {
        // Change selection to previous element if possible
        var index = selectable.SelectionIndex - 1;

        SelectedElement = index != -1 ? TryGetElement(index) as ISelectable : null;
      }

      selectable.SelectionIndex = -1;

      PointerClick.Unregister(args.Element);

      args.Element.GettingFocus -= Item_GettingFocus;
    }

    if (CanDragItems)
      args.Element.DragStarting -= Item_DragStarting;
  }

  private void OnLosingFocus(UIElement _, LosingFocusEventArgs args)
  {
    if (args.NewFocusedElement is Popup or null)
      args.TryCancel();
    else if (args.NewFocusedElement is not FrameworkElement { DataContext: object item } || !((ItemsSource as IList)?.IndexOf(item) >= 0))
    {
      // Deselect element if the focus is outside of the repeater
      if (DeselectOnLosingFocus)
        SelectedElement = null;
    }
  }

  private void Item_GettingFocus(UIElement sender, GettingFocusEventArgs args)
  {
    if (sender is not ISelectable selectable)
    {
      args.TryCancel();
      return;
    }

    if (args.FocusState == FocusState.Pointer && selectable == SelectedElement)
      return; // Item click will use this

    if (args.FocusState == FocusState.Keyboard && args.Direction != FocusNavigationDirection.None)
      SelectedElement = selectable; // Keyboard navigation will use this
    else if (SelectedElement is UIElement selectionElement && selectable != SelectedElement)
    {
      // For some reason, the focus will go to the first element in the repeater when deleting the focused element.
      //  This will try to change the focus back to the right element
      args.TrySetNewFocusedElement(selectionElement);
    }
  }

  private void Item_Clicked(object? sender, PointerRoutedEventArgs e)
  {
    if (sender is not ISelectable selectable) return;

    if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonReleased)
    {
      SelectedElement = selectable;
      // The element needs to be focused for the keyboard navigation to work
      (selectable as UIElement)?.Focus(FocusState.Pointer);
    }
  }

  private void Item_DragStarting(UIElement sender, DragStartingEventArgs args)
    => DragItemsStarting?.Invoke((sender as FrameworkElement)?.DataContext, args);
}