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
  public DeckTestingCardViewBase()
  {
    PointerPressed += DeckTestingCardViewBase_PointerPressed;
    PointerReleased += OnPointerReleased;
    PointerMoved += OnPointerMoved;
  }

  protected override void HoverPreviewUpdate(FrameworkElement sender, PointerRoutedEventArgs e)
  {
    if (DeckTestingCardDrag.IsDragging) return; // Disable card preview if card is being dragged

    base.HoverPreviewUpdate(sender, e);
  }

  private void DeckTestingCardViewBase_PointerPressed(object sender, PointerRoutedEventArgs e)
  {
    if (e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
    {
      var pointerPosition = e.GetCurrentPoint(null).Position;

      //Can't use pointer capturing because it will prevent other elements to fire pointer events...
      //CapturePointer(e.Pointer);

      //... Instead use the window content's pointer events
      XamlRoot.Content.PointerMoved += OnPointerMoved;
      XamlRoot.Content.PointerReleased += OnPointerReleased;

      CardPreview.Change(this, new(XamlRoot) { Uri = null });

      DragCardPreview.Change(this, new(XamlRoot)
      {
        Uri = SelectedFaceUri,
        Coordinates = new((float)pointerPosition.X, (float)pointerPosition.Y),
      });

      DeckTestingCardDrag.Completed += OnDragCompleted;
      DeckTestingCardDrag.Ended += OnDragEnded;

      DeckTestingCardDrag.Start(Model);
    }
  }

  private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
  {
    // Cancel drag if dropped on the dragged item
    if (DeckTestingCardDrag.IsDragging && DeckTestingCardDrag.Item == Model)
      DeckTestingCardDrag.Cancel();
  }

  private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
  {
    if (!DeckTestingCardDrag.IsDragging) return;

    if (e.GetCurrentPoint(null).Properties.IsRightButtonPressed || !e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
      DeckTestingCardDrag.Cancel();
    else
    {
      var pointerPosition = e.GetCurrentPoint(null).Position;

      DragCardPreview.Change(this, new(XamlRoot)
      {
        Coordinates = new((float)pointerPosition.X, (float)pointerPosition.Y),
      });
    }
  }

  private void OnDragCompleted(DeckTestingMTGCard item)
  {
    if (!item.IsToken)
      (this.FindParentByType<ListViewBase>()?.ItemsSource as ObservableCollection<DeckTestingMTGCard>)?.Remove(item);
  }

  private void OnDragEnded()
  {
    XamlRoot.Content.PointerMoved -= OnPointerMoved;
    XamlRoot.Content.PointerReleased -= OnPointerReleased;
    DeckTestingCardDrag.Completed -= OnDragCompleted;
    DeckTestingCardDrag.Ended -= OnDragEnded;
  }
}
