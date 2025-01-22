using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;

namespace MTGApplication.Features.CardSearch.Views.Controls.CardView;
public sealed partial class CardSearchCardImageView : CardSearchCardViewBase
{
  public CardSearchCardImageView()
  {
    InitializeComponent();

    DragStarting += ImageView_DragStarting;
    DropCompleted += ImageView_DropCompleted;
  }

  private DragAndDrop<CardMoveArgs>? DragAndDrop => field ??= new()
  {
    AcceptMove = false,
  };

  private async void ImageView_DragStarting(Microsoft.UI.Xaml.UIElement sender, Microsoft.UI.Xaml.DragStartingEventArgs args)
  {
    var deferral = args.GetDeferral();

    DragAndDrop!.OnDragStarting(new CardMoveArgs(Model, 1), out var operation);

    // Set the drag UI to the image element of the dragged element
    args.DragUI.SetContentFromSoftwareBitmap(await GetDragUI(ImageElement), args.GetPosition(ImageElement));
    args.Data.RequestedOperation = operation;

    deferral.Complete();
  }

  private void ImageView_DropCompleted(Microsoft.UI.Xaml.UIElement sender, Microsoft.UI.Xaml.DropCompletedEventArgs args)
    => DragAndDrop?.DropCompleted();
}
