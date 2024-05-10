using System;
using System.Windows.Input;

namespace MTGApplication.General.Views;

public abstract class DragAndDrop<T>
{
  public static DragAndDrop<T> DragOrigin { get; set; }

  public bool AcceptMove { get; init; } = true;

  public ICommand AddCommand { get; init; }
  public ICommand RemoveCommand { get; init; }

  /// <summary>
  /// Action that will be invoked when an item would be copied to the target source.
  /// <para>The <see cref="ICommand"/> is the <see cref="AddCommand"/></para>
  /// <para>The <see cref="string"/> is the copied data</para>
  /// </summary>
  public Action<ICommand, string> OnCopy { get; init; }

  /// <summary> 
  /// Action that will be invoked when an item would be moved between sources.
  /// <para>The first <see cref="ICommand"/> is the <see cref="AddCommand"/></para>
  /// <para>The second <see cref="ICommand"/> is the <see cref="DragOrigin"/>'s <see cref="RemoveCommand"/></para>
  /// <para>The <see cref="string"/> is the moved data</para>
  /// </summary>
  public Action<ICommand, ICommand, string> OnMove { get; init; }

  public virtual void OnDragStarting() => DragOrigin = this;

  public virtual void OnCompleted() => DragOrigin = null;

  protected abstract string SerializeItem(T Item);
}
