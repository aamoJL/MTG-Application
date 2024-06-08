using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Views.DragAndDrop;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardSearch.Services;

public class DraggableMTGCardGridViewDialog(string title = "", string itemTemplate = "", string gridStyle = "") : DraggableGridViewDialog<DeckEditorMTGCard>(title, itemTemplate, gridStyle)
{
  protected override void DraggableGridViewDialog_DragItemsStarting(ContentDialog dialog, DragItemsStartingEventArgs e)
  {
    if (e.Items[0] is DeckEditorMTGCard item)
    {
      DragAndDrop<DeckEditorMTGCard>.Item = item;
      e.Data.RequestedOperation = DataPackageOperation.Copy;
    }
    base.DraggableGridViewDialog_DragItemsStarting(dialog, e);
  }
}