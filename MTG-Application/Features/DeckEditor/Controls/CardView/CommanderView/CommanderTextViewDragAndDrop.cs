using Microsoft.UI.Xaml;
using MTGApplication.General.Models.Card;
using MTGApplication.General.Services;
using MTGApplication.General.Views;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Features.DeckEditor;

public class CommanderTextViewDragAndDrop(IClassCopier<MTGCard> itemCopier) : DragAndDrop<MTGCard>(itemCopier)
{
  public void DragStarting(UIElement sender, DragStartingEventArgs e)
  {
    OnDragStarting((sender as BasicCardView<MTGCard>)?.Model, out var requestedOperation);

    e.Data.RequestedOperation = requestedOperation;
  }

  public void DragOver(object sender, DragEventArgs e) => DragOver(e);

  public async void Drop(object sender, DragEventArgs e)
  {
    var def = e.GetDeferral();

    await Drop(e.AcceptedOperation, e.DataView.Contains(StandardDataFormats.Text) ? await e.DataView.GetTextAsync() : string.Empty);

    def.Complete();
  }

  public void DropCompleted(UIElement sender, DropCompletedEventArgs args) => DropCompleted();

  public override void DragOver(DragEventArgs eventArgs)
  {
    base.DragOver(eventArgs);

    if (Item?.Info.TypeLine.Contains("Legendary", StringComparison.OrdinalIgnoreCase) is false)
    {
      eventArgs.AcceptedOperation = DataPackageOperation.None;
      eventArgs.DragUIOverride.Caption = "Invalid card";
    }
  }
}
