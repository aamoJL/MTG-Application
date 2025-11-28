using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.DragDrop;

namespace MTGApplication.General.Views.DragAndDrop;

public class DragAndDrop<T>()
{
  public static DragAndDrop<T>? DragOrigin { get; private set; }
  public static T? Item { get; private set; }

  public bool AcceptMove { get; set; } = true;
  public string CopyCaptionOverride { get; set; } = string.Empty;
  public string MoveCaptionOverride { get; set; } = string.Empty;
  public bool IsDropContentVisible { get; set; } = true;

  public Func<T, Task>? OnCopy { get; set; }
  public Func<T, Task>? OnBeginMoveTo { get; set; }
  public Action<T>? OnBeginMoveFrom { get; set; }
  public Action<T>? OnExecuteMove { get; set; }
  public Func<string, Task>? OnExternalImport { get; set; }

  public virtual void OnInternalDragStarting(T item, out DataPackageOperation requestedOperation)
  {
    if (item == null)
      requestedOperation = DataPackageOperation.None;
    else if (AcceptMove)
      requestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
    else
      requestedOperation = DataPackageOperation.Copy;

    Item = item;
    DragOrigin = this;
  }

  public virtual void DragOver(DragEventArgs eventArgs)
  {
    // Block dropping if the origin is the same or the item is invalid
    if (DragOrigin == this || (!eventArgs.DataView.Contains(StandardDataFormats.Text) && Item == null))
      return;

    // Change operation to 'Move' if the shift key is down and move is an accepted operation.
    eventArgs.AcceptedOperation = (eventArgs.Modifiers & DragDropModifiers.Shift) == DragDropModifiers.Shift && DragOrigin?.AcceptMove is true && Item != null
      ? DataPackageOperation.Move : DataPackageOperation.Copy;

    if (eventArgs.AcceptedOperation == DataPackageOperation.Move && !string.IsNullOrEmpty(MoveCaptionOverride))
      eventArgs.DragUIOverride.Caption = MoveCaptionOverride;
    else if (eventArgs.AcceptedOperation == DataPackageOperation.Copy && !string.IsNullOrEmpty(CopyCaptionOverride))
      eventArgs.DragUIOverride.Caption = CopyCaptionOverride;

    eventArgs.DragUIOverride.IsContentVisible = IsDropContentVisible;

    eventArgs.Handled = true;
  }

  public virtual async Task Drop(DataPackageOperation operation, string data)
  {
    if (data != string.Empty)
    {
      await OnExternalDrop(operation, data);
    }
    else
    {
      if (Item == null || DragOrigin == this
        || !((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy
          || (operation & DataPackageOperation.Move) == DataPackageOperation.Move))
      {
        return; // don't drop on the origin and only allow copy and move operations
      }

      await OnInternalDrop(operation, Item);
    }
  }

  protected virtual async Task OnInternalDrop(DataPackageOperation operation, T item)
  {
    if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
    {
      if (OnCopy != null)
        await OnCopy.Invoke(item);
    }
    else if ((operation & DataPackageOperation.Move) == DataPackageOperation.Move)
    {
      DragOrigin?.OnBeginMoveFrom?.Invoke(item);

      if (OnBeginMoveTo != null)
        await OnBeginMoveTo.Invoke(item);

      OnExecuteMove?.Invoke(item);
      DragOrigin?.OnExecuteMove?.Invoke(item);
    }
  }

  protected virtual async Task OnExternalDrop(DataPackageOperation operation, string data)
  {
    if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
      if (OnExternalImport != null)
        await OnExternalImport.Invoke(data);
  }
}