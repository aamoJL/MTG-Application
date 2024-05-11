using System.Collections.Generic;

namespace MTGApplication.General.Services;

public interface IClassCopier<T>
{
  public T Copy(T item);
  public IEnumerable<T> Copy(IEnumerable<T> item);
}
