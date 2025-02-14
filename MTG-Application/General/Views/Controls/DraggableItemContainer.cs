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
    DragStarting += OnDragStarting;
  }

  private PointerPoint? DragPointerPoint { get; set; }

  private void OnPointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    => DragPointerPoint = e.GetCurrentPoint(Child);

  private void OnDragStarting(UIElement sender, DragStartingEventArgs args)
  {
    args.Cancel = true; // Cancel drag on this container...

    // ...and start drag on the child element instead
    if (Child != null && DragPointerPoint != null)
      _ = Child.StartDragAsync(DragPointerPoint);
  }
}
