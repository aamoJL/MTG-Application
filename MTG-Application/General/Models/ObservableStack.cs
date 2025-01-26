using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace MTGApplication.General.Models;

/// <inheritdoc cref="Stack{T}"/>
public class ObservableStack<T> : Stack<T>, INotifyCollectionChanged
{
  /// <inheritdoc cref="Stack{T}.Clear"/>
  public new void Clear()
  {
    base.Clear();

    CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Reset));
  }

  /// <inheritdoc cref="Stack{T}.Pop"/>
  public new T Pop()
  {
    var item = base.Pop();

    CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, item));

    return item;
  }

  /// <inheritdoc cref="Stack{T}.Push(T)"/>
  public new void Push(T item)
  {
    base.Push(item);

    CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, item));
  }

  /// <inheritdoc cref="Stack{T}.TryPop(out T)"/>
  public new bool TryPop([MaybeNullWhen(false)] out T result)
  {
    if (base.TryPop(out result))
    {
      CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, result));

      return true;
    }

    return false;
  }

  public event NotifyCollectionChangedEventHandler? CollectionChanged;
}
