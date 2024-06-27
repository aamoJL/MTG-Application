using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models;
using MTGApplication.General.Views.DragAndDrop;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardSearch.Services;

public class DraggableMTGCardGridViewDialog(string title = "", string itemTemplate = "", string gridStyle = "") : DraggableGridViewDialog<MTGCard>(title, itemTemplate, gridStyle)
{
  protected override void DraggableGridViewDialog_DragItemsStarting(ContentDialog dialog, DragItemsStartingEventArgs e)
  {
    if (e.Items[0] is MTGCard card)
    {
      DragAndDrop<CardMoveArgs>.Item = new(card);
      e.Data.RequestedOperation = DataPackageOperation.Copy;
    }
    base.DraggableGridViewDialog_DragItemsStarting(dialog, e);
  }
}