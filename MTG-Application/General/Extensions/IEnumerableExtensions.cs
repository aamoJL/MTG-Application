using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MTGApplication.General.Extensions;

public static class IEnumerableExtensions
{
  public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
     => self.Select((item, index) => (item, index));

  /// <summary>
  /// Returns position the given item would be on a sorted list
  /// </summary>
  public static int FindPosition<T>(this IEnumerable<T> items, T item, IComparer<T>? comparer = null)
  {
    comparer ??= Comparer<T>.Default;
    var low = 0;
    var high = items.Count();
    var index = high / 2;

    while (low < high)
    {
      if (comparer.Compare(items.ElementAt(index), item) < 0)
        low = index + 1;
      else
        high = index;

      index = low + (high - low) / 2;
    };

    return index;
  }

  public static bool TryFindIndex<T>(this ObservableCollection<T> self, Predicate<T> predicate, out int index)
  {
    index = -1;

    for (var i = 0; i < self.Count; i++)
    {
      if (predicate(self[i]))
      {
        index = i;
        return true;
      }
    }

    return false;
  }
}
