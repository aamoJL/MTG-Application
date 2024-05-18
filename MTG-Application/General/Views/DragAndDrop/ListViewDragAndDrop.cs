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
    OnDragStarting(e.Items[0] as MTGCard, out var requestedOperation);

    e.Data.RequestedOperation = requestedOperation;
  }

  public void DragOver(object sender, DragEventArgs e) => DragOver(e);

  public async void Drop(object sender, DragEventArgs e)
  {
    var def = e.GetDeferral();

    Drop(e.AcceptedOperation, e.DataView.Contains(StandardDataFormats.Text) ? await e.DataView.GetTextAsync() : string.Empty);

    def.Complete();
  }

  public void DragCompleted(ListViewBase sender, DragItemsCompletedEventArgs e) => DropCompleted();
}