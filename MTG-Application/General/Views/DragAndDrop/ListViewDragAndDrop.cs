using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.General.Views;

public class ListViewDragAndDrop : DragAndDrop<MTGCard>
{
  public ListViewDragAndDrop(IClassCopier<MTGCard> itemCopier) : base(itemCopier) { }

  public void DragStarting(object sender, DragItemsStartingEventArgs e)
  {
    if (e.Items[0] is MTGCard item)
    {
      OnDragStarting(item);
      e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
    }
  }

  public void DragOver(object sender, DragEventArgs e)
  {
    if (DragOrigin == this || (!e.DataView.Contains(StandardDataFormats.Text) && Item == null))
      return; // Block dropping if the origin is the same or the item is invalid

    // Change operation to 'Move' if the shift key is down and move is an accepted operation
    e.AcceptedOperation =
      ((e.Modifiers & Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift)
      == Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift
      && DragOrigin?.AcceptMove is true
      && Item != null) // Block Move operation if the drag is not from inside the app
      ? DataPackageOperation.Move : DataPackageOperation.Copy;
  }

  public async void Drop(object sender, DragEventArgs e)
  {
    var operation = e.AcceptedOperation;

    if (DragOrigin == this
      || !((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy
        || (operation & DataPackageOperation.Move) == DataPackageOperation.Move))
      return; // don't drop on the origin and only allow copy and move operations

    if (Item != null)
    {
      if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
      {
        OnCopy?.Invoke(Item);
      }
      else if ((operation & DataPackageOperation.Move) == DataPackageOperation.Move)
      {
        if (DataContext == null || DataContext != DragOrigin.DataContext)
        {
          // Can't move between different datacontexts,
          // instead the item will be copied to the target source and removed from the origin source
          OnCopy?.Invoke(ItemCopier.Copy(Item));
          DragOrigin?.OnRemove?.Invoke(Item);
        }
        else
        {
          DragOrigin?.OnBeginMoveFrom?.Invoke(Item);
          OnBeginMoveTo?.Invoke(Item);
          OnExecuteMove?.Invoke(Item);
        }
      }
    }
    else
    {
      if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
      {
        var def = e.GetDeferral();

        var data = await e.DataView.GetTextAsync();
        OnExternalImport?.Invoke(data);

        def.Complete();
      }
    }
  }

  public void DragCompleted(ListViewBase sender, DragItemsCompletedEventArgs e) => OnCompleted();
}