using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace MTGApplication.General.Extensions;

public static class ObservableCollectionExtensions
{
  public static IEnumerable<T> AddedItems<T>(this NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
      return e.NewItems.OfType<T>();
    else
      return [];
  }

  public static IEnumerable<T> RemovedItems<T>(this NotifyCollectionChangedEventArgs e)
  {
    if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
      return e.OldItems.OfType<T>();
    else
      return [];
  }
}