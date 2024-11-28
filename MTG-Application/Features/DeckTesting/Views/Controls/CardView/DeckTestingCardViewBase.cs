using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using MTGApplication.Features.DeckTesting.Models;
using MTGApplication.Features.DeckTesting.Services;
using MTGApplication.General.Views.Controls;
using MTGApplication.General.Views.Extensions;
using System.Collections.ObjectModel;

namespace MTGApplication.Features.DeckTesting.Views.Controls.CardView;

public partial class DeckTestingCardViewBase : BasicCardView<DeckTestingMTGCard>
{
  protected override void OnPointerEntered(PointerRoutedEventArgs e)
  {
    // Changing preview image on pointer enter will prevent flickering
    if (!DeckTestingCardDrag.IsDragging)
      DragCardPreview.Change(this, new(XamlRoot) { Uri = SelectedFaceUri });
  }

  protected override void OnPointerPressed(PointerRoutedEventArgs e)
  {
    if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
    {
      var pointerPosition = e.GetCurrentPoint(null).Position;

      //Can't use pointer capturing because it will prevent other elements to fire pointer events...
      //CapturePointer(e.Pointer);

      //... Instead use the window content's pointer events
      XamlRoot.Content.PointerMoved += Root_PointerMoved;
      XamlRoot.Content.PointerReleased += Root_PointerReleased;

      CardPreview.Change(this, new(XamlRoot) { Uri = string.Empty });

      DragCardPreview.Change(this, new(XamlRoot)
      {
        Uri = SelectedFaceUri,
        Coordinates = new((float)pointerPosition.X, (float)pointerPosition.Y),
        Offset = DragCardPreview.DefaultOffset,
        Opacity = DragCardPreview.DroppableOpacity,
      });

      DeckTestingCardDrag.Completed += OnDragCompleted;
      DeckTestingCardDrag.Ended += OnDragEnded;

      DeckTestingCardDrag.Start(Model);
    }
  }

  protected override void OnPointerReleased(PointerRoutedEventArgs e)
  {
    // Cancel drag if dropped on the dragged item
    if (DeckTestingCardDrag.IsDragging && DeckTestingCardDrag.Item == Model)
      DeckTestingCardDrag.Cancel();
  }

  protected override void OnPointerMoved(PointerRoutedEventArgs e)
  {
    // Disable Hover preview if card is being dragged
    if (!DeckTestingCardDrag.IsDragging)
      base.OnPointerMoved(e); // Updates hover preview
  }

  protected override void HoverPreviewUpdate(FrameworkElement sender, PointerRoutedEventArgs e)
  {
    if (DeckTestingCardDrag.IsDragging) return; // Disable card preview if card is being dragged

    base.HoverPreviewUpdate(sender, e);
  }

  protected virtual void Root_PointerReleased(object sender, PointerRoutedEventArgs e) => OnPointerReleased(e);

  protected virtual void Root_PointerMoved(object sender, PointerRoutedEventArgs e)
  {
    if (DeckTestingCardDrag.IsDragging
      && (e.GetCurrentPoint(null).Properties.IsRightButtonPressed
      || !e.GetCurrentPoint(null).Properties.IsLeftButtonPressed))
      DeckTestingCardDrag.Cancel();
  }

  private void OnDragCompleted(DeckTestingMTGCard? item)
  {
    if (item != null && !item.IsToken)
      (this.FindParentByType<ListViewBase>()?.ItemsSource as ObservableCollection<DeckTestingMTGCard>)?.Remove(item);
  }

  private void OnDragEnded()
  {
    XamlRoot.Content.PointerMoved -= Root_PointerMoved;
    XamlRoot.Content.PointerReleased -= Root_PointerReleased;
    DeckTestingCardDrag.Completed -= OnDragCompleted;
    DeckTestingCardDrag.Ended -= OnDragEnded;
  }
}
