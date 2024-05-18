using Microsoft.UI.Input.DragDrop;
using MTGApplication.General.Services;
using System;

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

  public Action<T> OnCopy { get; set; }
  public Action<T> OnBeginMoveTo { get; set; }
  public Action<T> OnBeginMoveFrom { get; set; }
  public Action<T> OnExecuteMove { get; set; }
  public Action<T> OnRemove { get; set; }
  public Action<string> OnExternalImport { get; set; }

  public virtual void OnDragStarting(T item)
  {
    Item = item;
    DragOrigin = this;
  }

  public virtual void OnCompleted()
  {
    Item = default;
    DragOrigin = null;
  }
}