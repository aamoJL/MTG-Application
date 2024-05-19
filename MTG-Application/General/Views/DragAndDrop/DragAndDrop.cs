using Microsoft.UI.Xaml;
using MTGApplication.General.Services;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.DragDrop;

namespace MTGApplication.General.Views;

public abstract class DragAndDrop<T>
{
  protected DragAndDrop(IClassCopier<T> itemCopier) => ItemCopier = itemCopier;

  public static DragAndDrop<T> DragOrigin { get; set; }
  public static T Item { get; set; }

  public IClassCopier<T> ItemCopier { get; }
  public bool AcceptMove { get; set; } = true;
  public string CopyCaptionOverride { get; set; } = string.Empty;
  public string MoveCaptionOverride { get; set; } = string.Empty;
  public bool IsContentVisible { get; set; } = true;

  public Func<T, Task> OnCopy { get; set; }
  public Func<T, Task> OnBeginMoveTo { get; set; }
  public Action<T> OnBeginMoveFrom { get; set; }
  public Action<T> OnExecuteMove { get; set; }
  public Action<T> OnRemove { get; set; }
  public Func<string, Task> OnExternalImport { get; set; }

  public virtual void OnDragStarting(T item, out DataPackageOperation requestedOperation)
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
    eventArgs.AcceptedOperation = ((eventArgs.Modifiers & DragDropModifiers.Shift) == DragDropModifiers.Shift && DragOrigin?.AcceptMove is true && Item != null)
      ? DataPackageOperation.Move : DataPackageOperation.Copy;

    if (eventArgs.AcceptedOperation == DataPackageOperation.Move && !string.IsNullOrEmpty(MoveCaptionOverride))
      eventArgs.DragUIOverride.Caption = MoveCaptionOverride;
    else if (eventArgs.AcceptedOperation == DataPackageOperation.Copy && !string.IsNullOrEmpty(CopyCaptionOverride))
      eventArgs.DragUIOverride.Caption = CopyCaptionOverride;

    eventArgs.DragUIOverride.IsContentVisible = IsContentVisible;
  }

  public virtual async Task Drop(DataPackageOperation operation, string data)
  {
    if (DragOrigin == this
      || !((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy
        || (operation & DataPackageOperation.Move) == DataPackageOperation.Move))
      return; // don't drop on the origin and only allow copy and move operations

    if (Item == null)
      await OnExternalDrop(operation, data);
    else
      await OnInternalDrop(operation, Item);
  }

  protected virtual async Task OnInternalDrop(DataPackageOperation operation, T item)
  {
    if (item == null) return;

    if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
    {
      await OnCopy?.Invoke(item);
    }
    else if ((operation & DataPackageOperation.Move) == DataPackageOperation.Move)
    {
      DragOrigin?.OnBeginMoveFrom?.Invoke(item);
      await OnBeginMoveTo?.Invoke(item);

      OnExecuteMove?.Invoke(item);
      DragOrigin?.OnExecuteMove?.Invoke(item);
    }
  }

  protected virtual async Task OnExternalDrop(DataPackageOperation operation, string data)
  {
    if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
      await OnExternalImport?.Invoke(data);
  }

  public virtual void DropCompleted()
  {
    Item = default;
    DragOrigin = null;
  }
}