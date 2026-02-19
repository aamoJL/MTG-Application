using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.General.Views.DragAndDrop;

public class ListViewDragAndDrop<TItem> : DragAndDrop<TItem> where TItem : class
{
  public void DragStarting(object _, DragItemsStartingEventArgs e)
  {
    if (e.Items[0] is not TItem item)
      throw new InvalidOperationException("Drag does not have any items");

    OnInternalDragStarting(item, out var requestedOperation);

    e.Data.RequestedOperation = requestedOperation;
  }

  public async void Drop(DragEventArgs e)
  {
    var def = e.GetDeferral();

    await Drop(e.AcceptedOperation, e.DataView.Contains(StandardDataFormats.Text) ? await e.DataView.GetTextAsync() : null);

    def.Complete();
  }
}