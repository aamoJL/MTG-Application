using Microsoft.UI.Xaml;
using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;

namespace MTGApplication.Features.CardSearch.Views.Controls.CardView;

public sealed partial class CardSearchCardImageView : CardSearchCardViewBase
{
  public CardSearchCardImageView() : base()
  {
    InitializeComponent();

    DragStarting += ImageView_DragStarting;
  }

  private DragAndDrop<MTGCard> DragAndDrop => field ??= new()
  {
    AcceptMove = false,
  };

  private async void ImageView_DragStarting(UIElement _, DragStartingEventArgs args)
  {
    var deferral = args.GetDeferral();

    DragAndDrop.OnInternalDragStarting(new(Model.Info), out var operation);

    // Set the drag UI to the image element of the dragged element
    args.DragUI.SetContentFromSoftwareBitmap(await DragAndDropHelpers.GetDragUI(ImageElement), args.GetPosition(ImageElement));
    args.Data.RequestedOperation = operation;

    deferral.Complete();
  }
}
