using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MTGApplication.General.Views.Controls;

/// <summary>
/// ItemContainer that can be used to relay DragStarting event to the item inside the container.
/// </summary>
public partial class DraggableItemContainer : ItemContainer
{
  public DraggableItemContainer()
  {
    PointerPressed += DeckEditorCardImageViewItemContainer_PointerPressed;
    DragStarting += DeckEditorCardImageViewItemContainer_DragStarting;
  }

  private PointerPoint? DragPointerPoint { get; set; }

  private void DeckEditorCardImageViewItemContainer_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    => DragPointerPoint = e.GetCurrentPoint(Child);

  private void DeckEditorCardImageViewItemContainer_DragStarting(UIElement sender, DragStartingEventArgs args)
  {
    if (Child != null && DragPointerPoint != null)
      _ = Child.StartDragAsync(DragPointerPoint);
  }
}
