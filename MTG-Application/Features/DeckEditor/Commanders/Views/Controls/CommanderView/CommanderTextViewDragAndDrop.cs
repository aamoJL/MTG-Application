using Microsoft.UI.Xaml;
using MTGApplication.Features.DeckEditor.Editor.Models;
using MTGApplication.General.Models;
using MTGApplication.General.Views.Controls;
using MTGApplication.General.Views.DragAndDrop;
using System;
using Windows.ApplicationModel.DataTransfer;

namespace MTGApplication.Features.DeckEditor.Commanders.Views.Controls.CommanderView;

public class CommanderTextViewDragAndDrop() : DragAndDrop<CardMoveArgs>()
{
  public void DragStarting(UIElement sender, DragStartingEventArgs e)
  {
    if ((sender as BasicCardView<DeckEditorMTGCard>)?.Model is not DeckEditorMTGCard item)
      return;

    OnInternalDragStarting(new(item, item.Count), out var requestedOperation);

    e.Data.RequestedOperation = requestedOperation;
  }

  public void DragOver(object _, DragEventArgs e) => DragOver(e);

  public async void Drop(object _, DragEventArgs e)
  {
    var def = e.GetDeferral();

    await Drop(e.AcceptedOperation, e.DataView.Contains(StandardDataFormats.Text) ? await e.DataView.GetTextAsync() : string.Empty);

    def.Complete();
  }

  public override void DragOver(DragEventArgs eventArgs)
  {
    base.DragOver(eventArgs);

    if (Item?.Card.Info.TypeLine.Contains("Legendary", StringComparison.OrdinalIgnoreCase) is false)
    {
      eventArgs.AcceptedOperation = DataPackageOperation.None;
      eventArgs.DragUIOverride.Caption = "Invalid card";
    }
  }
}
