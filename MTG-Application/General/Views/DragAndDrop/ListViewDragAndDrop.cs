using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.General.Models;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.General.Views.DragAndDrop;

public class ListViewDragAndDrop<TItem>(Func<TItem, CardMoveArgs> itemToArgsConverter) : DragAndDrop<CardMoveArgs>
{
  public void DragStarting(object _, DragItemsStartingEventArgs e)
  {
    OnDragStarting(itemToArgsConverter((TItem)e.Items[0]), out var requestedOperation);

    e.Data.RequestedOperation = requestedOperation;
  }

  public async void Drop(DragEventArgs e)
  {
    var def = e.GetDeferral();

    await Drop(e.AcceptedOperation, e.DataView.Contains(StandardDataFormats.Text) ? await e.DataView.GetTextAsync() : string.Empty);

    def.Complete();
  }

  public void DragCompleted(ListViewBase _, DragItemsCompletedEventArgs e) => DropCompleted();
}