using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.DragDrop;

namespace MTGApplication.General.Views.DragAndDrop;

[Obsolete]
public class DragAndDrop<T> where T : class
{
  public class ActiveDragArgs(DragAndDrop<T> origin)
  {
    public DragAndDrop<T> Origin { get; } = origin;
    public T? Item { get; init; } = null;
  }

  private static readonly DragDropModifiers _moveModifier = DragDropModifiers.Shift;

  public static ActiveDragArgs? ActiveDrag { get; set; } = null;

  public bool AcceptMove { get; set; } = true;
  public string CopyCaptionOverride { get; set; } = string.Empty;
  public string MoveCaptionOverride { get; set; } = string.Empty;
  public bool IsDropContentVisible { get; set; } = true;

  public Func<T, Task>? OnCopy { get; set; }
  public Func<T, Task>? OnBeginMoveTo { get; set; }
  public Action<T>? OnBeginMoveFrom { get; set; }
  public Action<T>? OnExecuteMove { get; set; }
  public Func<string, Task>? OnExternalImport { get; set; }
  public Action<Exception>? OnError { get; set; }

  public virtual void OnInternalDragStarting(T item, out DataPackageOperation requestedOperation)
  {
    if (item == null)
      requestedOperation = DataPackageOperation.None;
    else if (AcceptMove)
      requestedOperation = DataPackageOperation.Copy | DataPackageOperation.Move;
    else
      requestedOperation = DataPackageOperation.Copy;

    ActiveDrag = new(this) { Item = item };
  }

  public virtual void DragOver(DragEventArgs eventArgs)
  {
    // Block dropping if the origin is the same or the item is invalid
    if (ActiveDrag?.Origin == this) return;
    if (!eventArgs.DataView.Contains(StandardDataFormats.Text) && ActiveDrag?.Item is null) return;

    // Change operation to 'Move' if the shift key is down and move is an accepted operation.
    eventArgs.AcceptedOperation = (eventArgs.Modifiers & _moveModifier) == _moveModifier
      && ActiveDrag?.Origin.AcceptMove is true
      && ActiveDrag?.Item is not null
      ? DataPackageOperation.Move : DataPackageOperation.Copy;

    // Change UI Caption
    if (eventArgs.AcceptedOperation == DataPackageOperation.Move && !string.IsNullOrEmpty(MoveCaptionOverride))
      eventArgs.DragUIOverride.Caption = MoveCaptionOverride;
    else if (eventArgs.AcceptedOperation == DataPackageOperation.Copy && !string.IsNullOrEmpty(CopyCaptionOverride))
      eventArgs.DragUIOverride.Caption = CopyCaptionOverride;

    eventArgs.DragUIOverride.IsContentVisible = IsDropContentVisible;

    eventArgs.Handled = true;
  }

  public virtual async Task Drop(DataPackageOperation operation, string? data)
  {
    if (!string.IsNullOrEmpty(data))
      await OnExternalDrop(operation, data);
    else
    {
      if (ActiveDrag is null
        || ActiveDrag.Item is null
        || ActiveDrag.Origin == this
        || !((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy
          || (operation & DataPackageOperation.Move) == DataPackageOperation.Move))
        return; // don't drop on the origin and only allow copy and move operations

      await OnInternalDrop(operation, ActiveDrag.Item);
    }
  }

  protected virtual async Task OnInternalDrop(DataPackageOperation operation, T item)
  {
    try
    {
      if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
      {
        if (OnCopy != null)
          await OnCopy.Invoke(item);
      }
      else if ((operation & DataPackageOperation.Move) == DataPackageOperation.Move)
      {
        ActiveDrag?.Origin.OnBeginMoveFrom?.Invoke(item);

        if (OnBeginMoveTo != null)
          await OnBeginMoveTo.Invoke(item);

        OnExecuteMove?.Invoke(item);
        ActiveDrag?.Origin?.OnExecuteMove?.Invoke(item);
      }
    }
    catch (Exception e)
    {
      OnError?.Invoke(e);
    }
  }

  protected virtual async Task OnExternalDrop(DataPackageOperation operation, string data)
  {
    try
    {
      if ((operation & DataPackageOperation.Copy) == DataPackageOperation.Copy)
      {
        if (OnExternalImport != null)
          await OnExternalImport.Invoke(data);
      }
    }
    catch (Exception e)
    {
      OnError?.Invoke(e);
    }
  }
}