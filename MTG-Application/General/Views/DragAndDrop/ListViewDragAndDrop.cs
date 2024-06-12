using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Services;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.General.Views.DragAndDrop;

// TODO: Change to ImportResult.Card

public class ListViewDragAndDrop : DragAndDrop<DeckEditorMTGCard>
{
  public ListViewDragAndDrop(IClassCopier<DeckEditorMTGCard> itemCopier) : base(itemCopier) { }

  public void DragStarting(object sender, DragItemsStartingEventArgs e)
  {
    OnDragStarting(e.Items[0] as DeckEditorMTGCard, out var requestedOperation);

    e.Data.RequestedOperation = requestedOperation;
  }

  public void DragOver(object sender, DragEventArgs e) => DragOver(e);

  public async void Drop(object sender, DragEventArgs e)
  {
    var def = e.GetDeferral();

    await Drop(e.AcceptedOperation, e.DataView.Contains(StandardDataFormats.Text) ? await e.DataView.GetTextAsync() : string.Empty);

    def.Complete();
  }

  public void DragCompleted(ListViewBase sender, DragItemsCompletedEventArgs e) => DropCompleted();
}