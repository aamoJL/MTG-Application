using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGApplication.General.Models;

/// <summary>
/// List for record classes to enable equality check
/// </summary>
public class ValueEqualityList<T> : List<T>
{
  public override bool Equals(object? obj)
  {
    if (obj is not IEnumerable<T> enumerable) return false;

    return enumerable.SequenceEqual(this);
  }

  public override int GetHashCode()
  {
    var hashCode = 0;

    foreach (var item in this)
      if (item != null)
        hashCode ^= item.GetHashCode();

    return hashCode;
  }
}