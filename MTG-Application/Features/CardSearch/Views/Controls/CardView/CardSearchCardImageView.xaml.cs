using Microsoft.UI.Xaml;
using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Features.CardSearch.Views.Controls.CardView;

public sealed partial class CardSearchCardImageView : CardSearchCardViewBase
{
  public CardSearchCardImageView() : base()
  {
    InitializeComponent();

    DragStarting += ImageView_DragStarting;
  }

  private async void ImageView_DragStarting(UIElement _, DragStartingEventArgs e)
  {
    var deferral = e.GetDeferral();

    e.Data.RequestedOperation = DataPackageOperation.Copy;
    e.Data.Properties.Add(nameof(CardDragArgs), new CardDragArgs(new MTGCard(Model.Info), origin: this));

    // Set the drag UI to the image element of the dragged element
    e.DragUI.SetContentFromSoftwareBitmap(await DragAndDropHelpers.GetDragUI(ImageElement), e.GetPosition(ImageElement));

    deferral.Complete();
  }
}
