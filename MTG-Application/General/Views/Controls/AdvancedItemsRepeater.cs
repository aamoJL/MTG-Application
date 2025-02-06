using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Collections;

namespace MTGApplication.General.Views.Controls;

public partial class AdvancedItemsRepeater : ItemsRepeater
{
  public static readonly DependencyProperty SelectionModeProperty =
      DependencyProperty.Register(nameof(SelectionMode), typeof(ListViewSelectionMode), typeof(AdvancedItemsRepeater), new PropertyMetadata(ListViewSelectionMode.None));

  public AdvancedItemsRepeater() : base()
  {
    ElementPrepared += AdvancedItemsRepeater_ElementPrepared;
    ElementClearing += AdvancedItemsRepeater_ElementClearing;

    PointerClick.Clicked += ItemContainer_Clicked;
  }

  public ListViewSelectionMode SelectionMode
  {
    get => (ListViewSelectionMode)GetValue(SelectionModeProperty);
    set => SetValue(SelectionModeProperty, value);
  }

  public object? SelectedItem
  {
    get
    {
      if (SelectedContainer == null)
        return null;

      var index = GetElementIndex(SelectedContainer);

      if (index == -1)
        return null;

      return (ItemsSource as IList)?[index];
    }
  }

  protected ItemContainer? SelectedContainer
  {
    get;
    set
    {
      if (field == value || SelectionMode == ListViewSelectionMode.None)
        return;

      if (field != null)
        field.IsSelected = false;

      field = value;

      if (field != null)
      {
        field.IsSelected = true;
        field.StartBringIntoView();
      }
    }
  }
  protected PointerClick PointerClick { get; } = new();

  public void DeselectAll() => SelectedContainer = null;

  public void SelectItem(object item)
  {
    if (ItemsSourceView.IndexOf(item) is int index && index == -1)
      return;

    if (TryGetElement(index) is ItemContainer container)
      container.Focus(FocusState.Programmatic);
  }

  private void AdvancedItemsRepeater_ElementPrepared(ItemsRepeater sender, ItemsRepeaterElementPreparedEventArgs args)
  {
    if (args.Element is ItemContainer container)
    {
      // Select the prepared item if it has focus
      //  For example, if the user redo remove command, the container will have focus
      if (container.FocusState != FocusState.Unfocused)
        SelectedContainer = container;

      PointerClick.Register(container);
      container.GotFocus += ItemContainer_GotFocus;
    }
  }

  private void AdvancedItemsRepeater_ElementClearing(ItemsRepeater sender, ItemsRepeaterElementClearingEventArgs args)
  {
    if (args.Element is ItemContainer container)
    {
      if (container == SelectedContainer)
        DeselectAll();

      PointerClick.Unregister(container);
      container.GotFocus -= ItemContainer_GotFocus;
    }
  }

  private void ItemContainer_GotFocus(object sender, RoutedEventArgs e)
  {
    if (sender is not ItemContainer container)
      return;

    SelectedContainer = container;
  }

  private void ItemContainer_Clicked(object? sender, PointerRoutedEventArgs e)
    => (sender as ItemContainer)?.Focus(FocusState.Programmatic);
}
