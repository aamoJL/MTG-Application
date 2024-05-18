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
    OnDragStarting((sender as BasicCardView)?.Model, out var requestedOperation);

    e.Data.RequestedOperation = requestedOperation;
  }

  public void DragOver(object sender, DragEventArgs e) => DragOver(e);

  public async void Drop(object sender, DragEventArgs e)
  {
    var def = e.GetDeferral();

    Drop(e.AcceptedOperation, e.DataView.Contains(StandardDataFormats.Text) ? await e.DataView.GetTextAsync() : string.Empty);

    def.Complete();
  }

  public void DropCompleted(UIElement sender, DropCompletedEventArgs args) => DropCompleted();
}
