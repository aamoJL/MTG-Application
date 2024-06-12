using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models;
using MTGApplication.General.Services.Importers.CardImporter;
using MTGApplication.General.Views.DragAndDrop;
using Windows.ApplicationModel.DataTransfer;
using static MTGApplication.General.Services.ConfirmationService.DialogService;

namespace MTGApplication.Features.CardSearch.Services;

public class DraggableMTGCardGridViewDialog(string title = "", string itemTemplate = "", string gridStyle = "") : DraggableGridViewDialog<CardImportResult<MTGCardInfo>.Card>(title, itemTemplate, gridStyle)
{
  protected override void DraggableGridViewDialog_DragItemsStarting(ContentDialog dialog, DragItemsStartingEventArgs e)
  {
    if (e.Items[0] is CardImportResult<MTGCardInfo>.Card item)
    {
      DragAndDrop<CardImportResult<MTGCardInfo>.Card>.Item = item;
      e.Data.RequestedOperation = DataPackageOperation.Copy;
    }
    base.DraggableGridViewDialog_DragItemsStarting(dialog, e);
  }
}