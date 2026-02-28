using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Views.Controls;
using MTGApplication.General.Views.DragAndDrop;

namespace MTGApplication.Features.DeckEditor.Views.Controls.CardView;

public sealed partial class DeckCardImageViewContainer : ItemContainer, ISelectable
{
  public DeckCardImageViewContainer()
  {
    InitializeComponent();

    DragStarting += Container_DragStarting;
  }

  public int SelectionIndex { get; set; } = -1;

  private async void Container_DragStarting(Microsoft.UI.Xaml.UIElement _, Microsoft.UI.Xaml.DragStartingEventArgs args)
  {
    var deferral = args.GetDeferral();

    // Set the drag UI to the image element of the dragged element
    args.DragUI.SetContentFromSoftwareBitmap(
      await DragAndDropHelpers.GetDragUI(ImageView.ImageElement), args.GetPosition(ImageView.ImageElement));

    deferral.Complete();
  }
}
