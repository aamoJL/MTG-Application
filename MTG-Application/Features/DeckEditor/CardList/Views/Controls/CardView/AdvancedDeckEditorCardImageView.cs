using Microsoft.UI.Xaml;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;

namespace MTGApplication.Features.DeckEditor.CardList.Views.Controls.CardView;

public partial class AdvancedDeckEditorCardImageView : DeckEditorCardImageView
{
  public AdvancedDeckEditorCardImageView()
  {
    DragStarting += ImageView_DragStarting;
    DropCompleted += ImageView_DropCompleted;
  }

  protected DragAndDrop<CardMoveArgs>? DragAndDrop => field ??= new()
  {
    OnBeginMoveFrom = (item) => OnDropBeginMoveFrom?.Execute(item.Card as DeckEditorMTGCard ?? new DeckEditorMTGCard(item.Card.Info, item.Count)),
  };

  private async void ImageView_DragStarting(UIElement sender, DragStartingEventArgs args)
  {
    var deferral = args.GetDeferral();

    DragAndDrop!.OnDragStarting(new CardMoveArgs(Model, Model.Count), out var operation);

    // Set the drag UI to the image element of the dragged element
    args.DragUI.SetContentFromSoftwareBitmap(await GetDragUI(ImageElement), args.GetPosition(ImageElement));
    args.Data.RequestedOperation = operation;

    deferral.Complete();
  }

  private void ImageView_DropCompleted(UIElement sender, DropCompletedEventArgs args)
    => DragAndDrop!.DropCompleted();
}