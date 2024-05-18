using Microsoft.UI.Xaml;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services;
using MTGApplication.General.Views;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Features.DeckEditor;

public class BasicCardViewDragAndDrop : DragAndDrop<MTGCard>
{
  public BasicCardViewDragAndDrop(IClassCopier<MTGCard> itemCopier) : base(itemCopier) { }

  public void DragStarting(UIElement sender, DragStartingEventArgs e)
  {
    if (sender is BasicCardView view && view.Model != null)
    {
      OnDragStarting(view.Model);
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

    if (e.AcceptedOperation == DataPackageOperation.Move && !string.IsNullOrEmpty(MoveCaptionOverride))
      e.DragUIOverride.Caption = MoveCaptionOverride;
    else if (e.AcceptedOperation == DataPackageOperation.Copy && !string.IsNullOrEmpty(CopyCaptionOverride))
      e.DragUIOverride.Caption = CopyCaptionOverride;

    e.DragUIOverride.IsContentVisible = IsContentVisible;
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
        DragOrigin?.OnBeginMoveFrom?.Invoke(Item);
        OnBeginMoveTo?.Invoke(Item);

        OnExecuteMove?.Invoke(Item);
        DragOrigin?.OnExecuteMove?.Invoke(Item);
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

  public void DropCompleted(UIElement sender, DropCompletedEventArgs args) => OnCompleted();
}
