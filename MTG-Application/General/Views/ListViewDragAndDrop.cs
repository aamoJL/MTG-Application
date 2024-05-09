using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models.Card;
using System;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.General.Views;

public class ListViewDragAndDrop : DragAndDrop
{
  public void DragStarting(object sender, DragItemsStartingEventArgs e)
  {
    if (e.Items[0] is MTGCard card)
    {
      OnDragStarting();
      e.Data.SetText(JsonSerializer.Serialize(card));
      e.Data.RequestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
    }
  }

  public void DragOver(object sender, DragEventArgs e)
  {
    if (DragOrigin == this || !e.DataView.Contains(StandardDataFormats.Text))
      return; // Block dropping if the origin is the same or the item is invalid

    // Change operation to 'Move' if the shift key is down and move is an accepted operation
    e.AcceptedOperation =
      ((e.Modifiers & Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift)
      == Windows.ApplicationModel.DataTransfer.DragDrop.DragDropModifiers.Shift
      && DragOrigin?.AcceptMove is true)
      ? DataPackageOperation.Move : DataPackageOperation.Copy;
  }

  public async void Drop(object sender, DragEventArgs e)
  {
    var operation = e.AcceptedOperation;

    if (DragOrigin == this
      || !((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy
        || (operation & DataPackageOperation.Move) == DataPackageOperation.Move))
      return; // don't drop on the origin and only allow copy or move operations

    var def = e.GetDeferral();
    var data = await e.DataView.GetTextAsync();

    if (!string.IsNullOrEmpty(data))
    {
      if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
        OnCopy?.Invoke(AddCommand, data);
      else if ((operation & DataPackageOperation.Move) == DataPackageOperation.Move)
        OnMove?.Invoke(AddCommand, DragOrigin?.RemoveCommand, data);
    }

    def.Complete();
  }

  public void DragCompleted(ListViewBase sender, DragItemsCompletedEventArgs e) => OnCompleted();
}