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
    PointerPressed += OnPointerPressed;
    PointerReleased += DraggableItemContainer_PointerReleased;
    DragStarting += OnDragStarting;
    PointerMoved += DraggableItemContainer_PointerMoved;
  }

  private PointerPoint? DragPointerPoint { get; set; }

  private void OnPointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (CanDrag)
      DragPointerPoint = e.GetCurrentPoint(Child);
  }

  private void DraggableItemContainer_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    => DragPointerPoint = null;

  private void OnDragStarting(UIElement sender, DragStartingEventArgs args)
    => args.Cancel = true; // Cancel the default drag on this container

  private void DraggableItemContainer_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
  {
    if (DragPointerPoint != null && Child != null)
    {
      _ = Child.StartDragAsync(DragPointerPoint); // Start drag on the child element

      DragPointerPoint = null;
    }
  }
}
